using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Insurance.Services;

public sealed class InsuranceService : CrudServiceBase
{
    private const string CompaniesRes = "api/insurance/companies";
    private const string PoliciesRes = "api/insurance/policies";
    private const string PreAuthsRes = "api/insurance/preauthorizations";
    private const string ClaimsRes = "api/insurance/claims";

    public InsuranceService(ApiClient api) : base(api) { }

    public Task<IReadOnlyList<InsuranceCompanyDto>> GetCompaniesAsync(CancellationToken ct = default)
        => Api.GetAsync<IReadOnlyList<InsuranceCompanyDto>>(CompaniesRes, ct);

    public Task<InsuranceCompanyDto> GetCompanyByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<InsuranceCompanyDto>(CompaniesRes, id, ct);

    public Task<InsuranceCompanyDto> CreateCompanyAsync(InsuranceCompanyInput input, CancellationToken ct = default)
        => CreateEntityAsync<InsuranceCompanyDto>(CompaniesRes, input, ct);

    public Task UpdateCompanyAsync(string id, InsuranceCompanyInput input, CancellationToken ct = default)
        => UpdateEntityAsync(CompaniesRes, id, input, ct);

    public Task DeleteCompanyAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(CompaniesRes, id, ct);

    public Task<PagedResult<InsurancePolicyListItemDto>> GetPoliciesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<InsurancePolicyListItemDto>(PoliciesRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<InsurancePolicyDto> GetPolicyByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<InsurancePolicyDto>(PoliciesRes, id, ct);

    public Task<IReadOnlyList<InsurancePolicyDto>> GetPoliciesByPatientAsync(string patientId, CancellationToken ct = default)
        => Api.GetAsync<IReadOnlyList<InsurancePolicyDto>>($"{PoliciesRes}/by-patient/{patientId}", ct);

    public Task<InsurancePolicyDto> CreatePolicyAsync(InsurancePolicyInput input, CancellationToken ct = default)
        => CreateEntityAsync<InsurancePolicyDto>(PoliciesRes, input, ct);

    public Task<InsurancePolicyDto> UpdateCoverageAsync(string id, UpdatePolicyCoverageInput input, CancellationToken ct = default)
        => Api.PutAsync<InsurancePolicyDto>($"{PoliciesRes}/{id}/coverage", input, ct);

    public Task<InsurancePolicyDto> ChangePolicyStatusAsync(string id, string action, CancellationToken ct = default)
        => Api.PostAsync<InsurancePolicyDto>($"{PoliciesRes}/{id}/status", new ChangePolicyStatusInput { Action = action }, ct);

    public Task<IReadOnlyList<PolicyDocumentDto>> GetPolicyDocumentsAsync(string policyId, CancellationToken ct = default)
        => Api.GetAsync<IReadOnlyList<PolicyDocumentDto>>($"{PoliciesRes}/{policyId}/documents", ct);

    public Task<PolicyDocumentDto> UploadPolicyDocumentAsync(
        string policyId,
        string type,
        string fileName,
        string contentType,
        Stream stream,
        CancellationToken ct = default)
    {
        var fields = new Dictionary<string, string> { ["type"] = type };
        return Api.PostFormAsync<PolicyDocumentDto>(
            $"{PoliciesRes}/{policyId}/documents",
            fields,
            ("file", fileName, contentType, stream),
            ct);
    }

    public Task<byte[]> DownloadPolicyDocumentAsync(string policyId, string documentId, CancellationToken ct = default)
        => Api.GetBytesAsync($"{PoliciesRes}/{policyId}/documents/{documentId}/content", ct);

    public Task DeletePolicyDocumentAsync(string policyId, string documentId, CancellationToken ct = default)
        => Api.SendDeleteAsync($"{PoliciesRes}/{policyId}/documents/{documentId}", ct);

    public Task<PagedResult<PreAuthorizationListItemDto>> GetPreAuthorizationsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<PreAuthorizationListItemDto>(PreAuthsRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<PreAuthorizationDto> GetPreAuthorizationByExaminationAsync(string examinationId, CancellationToken ct = default)
        => Api.GetAsync<PreAuthorizationDto>($"{PreAuthsRes}/by-examination/{examinationId}", ct);

    public Task<PreAuthorizationDto> CreatePreAuthorizationAsync(CreatePreAuthorizationInput input, CancellationToken ct = default)
        => CreateEntityAsync<PreAuthorizationDto>(PreAuthsRes, input, ct);

    public Task<PreAuthorizationDto> DecidePreAuthorizationAsync(string id, DecidePreAuthorizationInput input, CancellationToken ct = default)
        => Api.PostAsync<PreAuthorizationDto>($"{PreAuthsRes}/{id}/decide", input, ct);

    public Task<IReadOnlyList<PreAuthorizationDocumentDto>> GetPreAuthorizationDocumentsAsync(string id, CancellationToken ct = default)
        => Api.GetAsync<IReadOnlyList<PreAuthorizationDocumentDto>>($"{PreAuthsRes}/{id}/documents", ct);

    public Task<PreAuthorizationDocumentDto> UploadPreAuthorizationDocumentAsync(
        string id,
        string type,
        string fileName,
        string contentType,
        Stream stream,
        CancellationToken ct = default)
    {
        var fields = new Dictionary<string, string> { ["type"] = type };
        return Api.PostFormAsync<PreAuthorizationDocumentDto>(
            $"{PreAuthsRes}/{id}/documents",
            fields,
            ("file", fileName, contentType, stream),
            ct);
    }

    public Task<byte[]> DownloadPreAuthorizationDocumentAsync(string id, string documentId, CancellationToken ct = default)
        => Api.GetBytesAsync($"{PreAuthsRes}/{id}/documents/{documentId}/content", ct);

    public Task DeletePreAuthorizationDocumentAsync(string id, string documentId, CancellationToken ct = default)
        => Api.SendDeleteAsync($"{PreAuthsRes}/{id}/documents/{documentId}", ct);

    public Task<PagedResult<ClaimListItemDto>> GetClaimsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<ClaimListItemDto>(ClaimsRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<ClaimDto> GetClaimByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<ClaimDto>(ClaimsRes, id, ct);

    public Task<ClaimDto> GetClaimByExaminationAsync(string examinationId, CancellationToken ct = default)
        => Api.GetAsync<ClaimDto>($"{ClaimsRes}/by-examination/{examinationId}", ct);

    public Task<ClaimDto> CreateClaimAsync(CreateClaimInput input, CancellationToken ct = default)
        => CreateEntityAsync<ClaimDto>(ClaimsRes, input, ct);

    public Task<ClaimDto> SubmitClaimAsync(string id, CancellationToken ct = default)
        => Api.PostAsync<ClaimDto>($"{ClaimsRes}/{id}/submit", null, ct);

    public Task<ClaimDto> AdjudicateClaimAsync(string id, AdjudicateClaimInput input, CancellationToken ct = default)
        => Api.PostAsync<ClaimDto>($"{ClaimsRes}/{id}/adjudicate", input, ct);

    public Task<ClaimDto> ResubmitClaimAsync(string id, CancellationToken ct = default)
        => Api.PostAsync<ClaimDto>($"{ClaimsRes}/{id}/resubmit", null, ct);

    public Task<ClaimDto> RecordSettlementAsync(string id, RecordSettlementInput input, CancellationToken ct = default)
        => Api.PostAsync<ClaimDto>($"{ClaimsRes}/{id}/settlements", input, ct);

    public Task<InsuranceStatsDto> GetStatsAsync(CancellationToken ct = default)
        => Api.GetAsync<InsuranceStatsDto>("api/insurance/stats", ct);
}
