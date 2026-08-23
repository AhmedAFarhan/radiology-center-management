using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Application.DTOs;

namespace RadiologyCenter.Catalog.Application.Queries.ExportExaminationTypes;

public static class ExportExaminationTypesQueryHandler
{
    private const int MaxExportRows = 50_000;

    public static async Task<Result<FileContentDto>> HandleAsync(
        ExportExaminationTypesQuery query,
        IExaminationTypeRepository examinationTypeRepository,
        IExcelService excelService,
        CancellationToken ct)
    {
        var request = WithIsActiveFilter(WithMaxRows(query.Request), query.IsActive);
        var paged = await examinationTypeRepository.GetPagedAsync(request, ct);
        var dtos = paged.Items.Select(t => t.Adapt<ExaminationTypeDto>()).ToList();

        var content = excelService.Export(
            "ExaminationTypes",
            "examination-types.xlsx",
                Columns,
            dtos);

        return Result.Success(new FileContentDto(content, "examination-types.xlsx", ExcelContentTypes.Xlsx));
    }

    private static IReadOnlyList<ExcelColumn<ExaminationTypeDto>> Columns { get; } =
    [
        new("Excel.ExamType.Code", "Code", t => t.Code, width: 16),
        new("Excel.ExamType.Name", "Name", t => t.Name, width: 32),
        new("Excel.ExamType.Modality", "Modality", t => t.Modality),
        new("Excel.ExamType.BodyPart", "Body Part", t => t.BodyPart, width: 24),
        new("Excel.ExamType.Duration", "Duration (min)", t => t.StandardDurationMinutes, ExcelColumnType.Number, width: 15),
        new("Excel.ExamType.Price", "Price", t => t.Price, ExcelColumnType.Currency, width: 14),
        new("Excel.ExamType.RequiresPreparation", "Requires Preparation", t => t.RequiresPreparation ? "Yes" : "No"),
        new("Excel.ExamType.RequiresConsent", "Requires Consent", t => t.RequiresConsent ? "Yes" : "No"),
        new("Excel.Common.IsActive", "Active", t => t.IsActive ? "Yes" : "No"),
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
