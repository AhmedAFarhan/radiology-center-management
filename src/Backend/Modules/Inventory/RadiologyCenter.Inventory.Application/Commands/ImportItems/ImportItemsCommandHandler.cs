using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.Commands.Common;
using RadiologyCenter.Inventory.Application.Commands.CreateItem;
using RadiologyCenter.Inventory.Domain.Entities;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Application.Commands.ImportItems;

/// <summary>
/// Best-effort import of inventory items from an uploaded template workbook.
/// Valid rows commit inside a single transaction; invalid rows are reported
/// per row with localized messages. Duplicate names (existing or repeated in
/// the file) are rejected.
/// </summary>
public static class ImportItemsCommandHandler
{
    public static async Task<Result<ExcelImportResult>> HandleAsync(
        ImportItemsCommand command,
        IValidator<CreateItemCommand> validator,
        IItemRepository itemRepository,
        IInventoryUnitOfWork unitOfWork,
        IExcelService excelService,
        CancellationToken ct)
    {
        if (command.FileContent.Length > ExcelImportLimits.MaxFileBytes)
            return Result.Failure<ExcelImportResult>(Error.Validation("Excel.FileTooLarge", Translate("Excel.FileTooLarge")));

        ParsedWorkbook parsed;
        try
        {
            using var stream = new MemoryStream(command.FileContent);
            parsed = excelService.ReadTemplate(stream, HeaderCodes);
        }
        catch (ExcelImportException ex)
        {
            return Result.Failure<ExcelImportResult>(Error.Validation(ex.Code, TranslateArgs(ex.Code, ex.Args)));
        }

        if (!string.IsNullOrWhiteSpace(parsed.TemplateVersion) && parsed.TemplateVersion != ExcelImportLimits.TemplateVersion)
            return Result.Failure<ExcelImportResult>(Error.Validation("Excel.WrongVersion", Translate("Excel.WrongVersion")));

        var existingNames = (await itemRepository.GetAllAsync(ct))
            .Select(i => Normalize(i.Name))
            .ToHashSet();

        var errors = new List<ExcelRowError>();
        var pending = new List<CreateItemCommand>();
        var seenInFile = new HashSet<string>();

        foreach (var row in parsed.Rows)
        {
            var name = row.Value(HeaderCode.Name)?.Trim() ?? string.Empty;
            var brand = row.Value(HeaderCode.Brand)?.Trim();
            var categoryText = row.Value(HeaderCode.Category)?.Trim() ?? string.Empty;
            var unitText = row.Value(HeaderCode.Unit)?.Trim() ?? string.Empty;
            var reorderLevelText = row.Value(HeaderCode.ReorderLevel)?.Trim();
            var reorderQuantityText = row.Value(HeaderCode.ReorderQuantity)?.Trim();
            var lotTracked = ParseYesNo(row.Value(HeaderCode.LotTracked));
            var storageInstructions = row.Value(HeaderCode.StorageInstructions)?.Trim();

            var rowErrors = new List<string>();

            if (!string.IsNullOrWhiteSpace(categoryText)
                && ItemCategory.GetAll<ItemCategory>().All(c => !c.Name.Equals(categoryText, StringComparison.OrdinalIgnoreCase)))
                rowErrors.Add(Translate("Excel.Error.InvalidCategory"));

            if (!string.IsNullOrWhiteSpace(unitText)
                && UnitType.GetAll<UnitType>().All(u => !u.Name.Equals(unitText, StringComparison.OrdinalIgnoreCase)))
                rowErrors.Add(Translate("Excel.Error.InvalidUnit"));

            if (reorderLevelText is { Length: > 0 }
                && (!int.TryParse(reorderLevelText, out var reorderLevel) || reorderLevel < 0))
                rowErrors.Add(Translate("Excel.Error.InvalidNumber"));

            if (reorderQuantityText is { Length: > 0 }
                && (!int.TryParse(reorderQuantityText, out var reorderQuantity) || reorderQuantity < 0))
                rowErrors.Add(Translate("Excel.Error.InvalidNumber"));

            if (rowErrors.Count == 0)
            {
                var candidate = new CreateItemCommand(
                    name,
                    categoryText,
                    unitText,
                    brand,
                    int.TryParse(reorderLevelText, out var level) ? level : 0,
                    int.TryParse(reorderQuantityText, out var quantity) ? quantity : 0,
                    lotTracked,
                    storageInstructions);

                var validation = await validator.ValidateAsync(candidate, ct);
                rowErrors.AddRange(validation.Errors.Select(e => Translator.LocalizeCode(e.ErrorCode, e.ErrorMessage)));

                var normalizedName = Normalize(name);
                if (existingNames.Contains(normalizedName) || seenInFile.Contains(normalizedName))
                    rowErrors.Add(Translate("Excel.Error.DuplicateName"));
                else
                    seenInFile.Add(normalizedName);

                if (rowErrors.Count == 0)
                    pending.Add(candidate);
            }

            if (rowErrors.Count > 0)
                errors.Add(new ExcelRowError(row.RowNumber, "Excel.Error.RowInvalid", string.Join(" · ", rowErrors)));
        }

        if (pending.Count > 0)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
            foreach (var candidate in pending)
            {
                await itemRepository.AddAsync(
                    Item.Create(
                        candidate.Name,
                        ItemCategory.FromName<ItemCategory>(candidate.Category),
                        UnitType.FromName<UnitType>(candidate.Unit),
                        candidate.Brand,
                        candidate.ReorderLevel,
                        candidate.ReorderQuantity,
                        candidate.LotTracked,
                        candidate.StorageInstructions),
                    ct);
            }

            await unitOfWork.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        return Result.Success(new ExcelImportResult(parsed.Rows.Count, pending.Count, errors));
    }

    private static class HeaderCode
    {
        public const string Name = "Excel.Item.Name";
        public const string Brand = "Excel.Item.Brand";
        public const string Category = "Excel.Item.Category";
        public const string Unit = "Excel.Item.Unit";
        public const string ReorderLevel = "Excel.Item.ReorderLevel";
        public const string ReorderQuantity = "Excel.Item.ReorderQuantity";
        public const string LotTracked = "Excel.Item.LotTracked";
        public const string StorageInstructions = "Excel.Item.StorageInstructions";
    }

    private static IReadOnlyList<string> HeaderCodes { get; } =
    [
        HeaderCode.Name,
        HeaderCode.Brand,
        HeaderCode.Category,
        HeaderCode.Unit,
        HeaderCode.ReorderLevel,
        HeaderCode.ReorderQuantity,
        HeaderCode.LotTracked,
        HeaderCode.StorageInstructions,
    ];

    private static bool ParseYesNo(string? value) =>
        value is not null && value.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase);

    private static string Normalize(string value) => value.Trim().ToLowerInvariant();

    private static string Translate(string code) => Translator.LocalizeCode(code, code);

    private static string TranslateArgs(string code, object[] args)
    {
        var template = Translate(code);
        try
        {
            return string.Format(template, args);
        }
        catch (FormatException)
        {
            return template;
        }
    }
}
