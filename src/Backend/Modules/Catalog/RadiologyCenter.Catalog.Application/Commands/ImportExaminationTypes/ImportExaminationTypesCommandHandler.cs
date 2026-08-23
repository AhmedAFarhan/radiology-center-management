using RadiologyCenter.BuildingBlocks.Application.Abstractions.Services;
using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Application.Commands.CreateExaminationType;
using RadiologyCenter.Catalog.Domain.Entities;
using RadiologyCenter.Catalog.Domain.Enumerations;

namespace RadiologyCenter.Catalog.Application.Commands.ImportExaminationTypes;

/// <summary>
/// Best-effort import of examination types from an uploaded template
/// workbook. Valid rows commit inside a single transaction (codes generated
/// via the shared sequence); invalid rows are reported per row with
/// localized messages.
/// </summary>
public static class ImportExaminationTypesCommandHandler
{
    public static async Task<Result<ExcelImportResult>> HandleAsync(
        ImportExaminationTypesCommand command,
        IValidator<CreateExaminationTypeCommand> validator,
        IExaminationTypeRepository examinationTypeRepository,
        INumberSequenceGenerator numberSequenceGenerator,
        ICatalogUnitOfWork unitOfWork,
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

        var existingNames = (await examinationTypeRepository.GetAllAsync(ct))
            .Select(t => Normalize(t.Name))
            .ToHashSet();

        var errors = new List<ExcelRowError>();
        var pending = new List<CreateExaminationTypeCommand>();
        var seenInFile = new HashSet<string>();

        foreach (var row in parsed.Rows)
        {
            var name = row.Value(HeaderCode.Name)?.Trim() ?? string.Empty;
            var modalityText = row.Value(HeaderCode.Modality)?.Trim();
            var bodyPart = row.Value(HeaderCode.BodyPart)?.Trim() ?? string.Empty;
            var durationText = row.Value(HeaderCode.Duration)?.Trim();
            var priceText = row.Value(HeaderCode.Price)?.Trim();
            var preparation = ParseYesNo(row.Value(HeaderCode.RequiresPreparation));
            var consent = ParseYesNo(row.Value(HeaderCode.RequiresConsent));

            var rowErrors = new List<string>();

            if (!string.IsNullOrWhiteSpace(modalityText)
                && Modality.GetAll<Modality>().All(m => !m.Name.Equals(modalityText, StringComparison.OrdinalIgnoreCase)))
                rowErrors.Add(Translate("Excel.Error.InvalidModality"));

            if (durationText is { Length: > 0 }
                && (!int.TryParse(durationText, out var duration) || duration < 0))
                rowErrors.Add(Translate("Excel.Error.InvalidNumber"));

            if (priceText is { Length: > 0 }
                && (!decimal.TryParse(priceText, out var price) || price < 0))
                rowErrors.Add(Translate("Excel.Error.InvalidNumber"));

            if (rowErrors.Count == 0)
            {
                var candidate = new CreateExaminationTypeCommand(
                    name,
                    modalityText!,
                    bodyPart,
                    int.TryParse(durationText, out var parsedDuration) ? parsedDuration : 0,
                    decimal.TryParse(priceText, out var parsedPrice) ? parsedPrice : 0m,
                    preparation,
                    consent);

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
                var code = await numberSequenceGenerator.GenerateNextAsync(
                    "ExaminationType",
                    "EXM",
                    4,
                    transaction.DbTransaction,
                    ct);

                await examinationTypeRepository.AddAsync(
                    ExaminationType.Create(
                        code,
                        candidate.Name,
                        Modality.FromName<Modality>(candidate.Modality),
                        candidate.BodyPart,
                        candidate.StandardDurationMinutes,
                        candidate.Price,
                        candidate.RequiresPreparation,
                        candidate.RequiresConsent),
                    ct);
            }

            await transaction.CommitAsync(ct);
        }

        return Result.Success(new ExcelImportResult(parsed.Rows.Count, pending.Count, errors));
    }

    private static class HeaderCode
    {
        public const string Name = "Excel.ExamType.Name";
        public const string Modality = "Excel.ExamType.Modality";
        public const string BodyPart = "Excel.ExamType.BodyPart";
        public const string Duration = "Excel.ExamType.Duration";
        public const string Price = "Excel.ExamType.Price";
        public const string RequiresPreparation = "Excel.ExamType.RequiresPreparation";
        public const string RequiresConsent = "Excel.ExamType.RequiresConsent";
    }

    private static IReadOnlyList<string> HeaderCodes { get; } =
    [
        HeaderCode.Name,
        HeaderCode.Modality,
        HeaderCode.BodyPart,
        HeaderCode.Duration,
        HeaderCode.Price,
        HeaderCode.RequiresPreparation,
        HeaderCode.RequiresConsent,
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
