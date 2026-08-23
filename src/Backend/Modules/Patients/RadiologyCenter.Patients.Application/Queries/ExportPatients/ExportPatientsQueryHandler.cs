using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Patients.Application.Abstractions;
using RadiologyCenter.Patients.Application.DTOs;

namespace RadiologyCenter.Patients.Application.Queries.ExportPatients;

public static class ExportPatientsQueryHandler
{
    private const int MaxExportRows = 50_000;

    public static async Task<Result<FileContentDto>> HandleAsync(
        ExportPatientsQuery query,
        IPatientRepository patientRepository,
        IExcelService excelService,
        CancellationToken ct)
    {
        var request = WithIsActiveFilter(WithMaxRows(query.Request), query.IsActive);
        var paged = await patientRepository.GetPagedAsync(request, ct);
        var dtos = paged.Items.Select(t => t.Adapt<PatientDto>()).ToList();

        var content = excelService.Export(
            "Patients",
            "patients.xlsx",
            Columns,
            dtos);

        return Result.Success(new FileContentDto(content, "patients.xlsx", ExcelContentTypes.Xlsx));
    }

    private static IReadOnlyList<ExcelColumn<PatientDto>> Columns { get; } =
    [
        new("Excel.Patient.Code", "Code", p => p.PatientCode, width: 14),
        new("Excel.Patient.FullName", "Full Name", p => p.FullName, width: 34),
        new("Excel.Patient.Gender", "Gender", p => p.Gender),
        new("Excel.Patient.DateOfBirth", "Date of Birth", p => p.DateOfBirth, ExcelColumnType.Date, width: 16),
        new("Excel.Patient.Age", "Age", p => p.Age, ExcelColumnType.Number, width: 8),
        new("Excel.Patient.PhoneNumber", "Phone", p => p.PhoneNumber, width: 18),
        new("Excel.Patient.Email", "Email", p => p.Email, width: 26),
        new("Excel.Patient.Address", "Address", p => p.Address, width: 32),
        new("Excel.Patient.NationalId", "National ID", p => p.NationalId, width: 18),
        new("Excel.Patient.BloodType", "Blood Type", p => p.BloodType),
        new("Excel.Patient.Allergies", "Allergies", p => p.Allergies, width: 28),
        new("Excel.Patient.MedicalHistory", "Medical History", p => p.MedicalHistory, width: 32),
        new("Excel.Patient.ReferringPhysician", "Referring Physician", p => p.ReferringPhysician, width: 24),
        new("Excel.Common.IsActive", "Active", p => p.IsActive ? "Yes" : "No"),
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
