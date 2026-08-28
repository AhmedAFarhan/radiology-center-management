using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Patients.Services;

public sealed class PatientService : CrudServiceBase
{
    private const string Res = "api/patients";

    public PatientService(ApiClient api) : base(api) { }

    public Task<PagedResult<PatientDto>> GetPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<PatientDto>(Res, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<PatientDto> GetByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<PatientDto>(Res, id, ct);

    public Task<PatientDto> CreateAsync(PatientInput input, CancellationToken ct = default)
        => CreateEntityAsync<PatientDto>(Res, input, ct);

    public Task UpdateAsync(string id, PatientInput input, CancellationToken ct = default)
        => UpdateEntityAsync(Res, id, input, ct);

    public Task ActivateAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(Res, id, true, ct);

    public Task DeactivateAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(Res, id, false, ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(Res, id, ct);

    public Task<byte[]> ExportAsync(string? searchTerm, CancellationToken ct = default)
        => Api.PostBytesAsync($"{Res}/export", new
        {
            searchTerm,
            pagination = new { pageNumber = 1, pageSize = 50_000 },
        }, ct);

    public Task<byte[]> DownloadImportTemplateAsync(CancellationToken ct = default)
        => Api.GetBytesAsync($"{Res}/import-template", ct);

    public Task<ExcelImportResultDto> ImportAsync(string fileName, Stream content, CancellationToken ct = default)
        => Api.PostFormAsync<ExcelImportResultDto>(
            $"{Res}/import",
            file: ("file", fileName, ExcelContentType, content),
            ct: ct);

    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
