using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Inventory.Application.Abstractions;
using RadiologyCenter.Inventory.Application.DTOs;

namespace RadiologyCenter.Inventory.Application.Queries.ExportItems;

public static class ExportItemsQueryHandler
{
    private const int MaxExportRows = 50_000;

    public static async Task<Result<FileContentDto>> HandleAsync(
        ExportItemsQuery query,
        IItemRepository itemRepository,
        IExcelService excelService,
        CancellationToken ct)
    {
        var request = WithIsActiveFilter(WithMaxRows(query.Request), query.IsActive);
        var paged = await itemRepository.GetPagedAsync(request, ct);
        var dtos = paged.Items.Select(i => i.Adapt<ItemDto>()).ToList();

        var content = excelService.Export(
            "Items",
            "inventory-items.xlsx",
            Columns,
            dtos);

        return Result.Success(new FileContentDto(content, "inventory-items.xlsx", ExcelContentTypes.Xlsx));
    }

    private static IReadOnlyList<ExcelColumn<ItemDto>> Columns { get; } =
    [
        new("Excel.Item.Name", "Name", i => i.Name, width: 32),
        new("Excel.Item.Brand", "Brand", i => i.Brand ?? string.Empty, width: 20),
        new("Excel.Item.Category", "Category", i => i.Category),
        new("Excel.Item.Unit", "Unit", i => i.Unit),
        new("Excel.Item.ReorderLevel", "Reorder Level", i => i.ReorderLevel, ExcelColumnType.Number, width: 14),
        new("Excel.Item.ReorderQuantity", "Reorder Quantity", i => i.ReorderQuantity, ExcelColumnType.Number, width: 16),
        new("Excel.Item.LotTracked", "Lot Tracked", i => i.LotTracked ? "Yes" : "No"),
        new("Excel.Item.StorageInstructions", "Storage Instructions", i => i.StorageInstructions ?? string.Empty, width: 36),
        new("Excel.Common.IsActive", "Active", i => i.IsActive ? "Yes" : "No"),
    ];

    private static QueryRequest WithMaxRows(QueryRequest request) => new()
    {
        Pagination = new PaginationParams { PageNumber = 1, PageSize = MaxExportRows },
        SortBy = request.SortBy,
        SortDescending = request.SortDescending,
        SearchTerm = request.SearchTerm,
        SearchFields = request.SearchFields,
        Filters = request.Filters,
    };

    private static QueryRequest WithIsActiveFilter(QueryRequest request, bool? isActive)
    {
        if (isActive is null)
            return request;

        var filters = (request.Filters ?? []).ToList();
        filters.Add(new FilterCriteria { Field = "IsActive", Operator = FilterOperator.Equals, Value = isActive.Value });

        return new QueryRequest
        {
            Pagination = request.Pagination,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending,
            SearchTerm = request.SearchTerm,
            SearchFields = request.SearchFields,
            Filters = filters
        };
    }
}
