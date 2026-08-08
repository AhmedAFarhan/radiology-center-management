using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class InsuranceService
{
    private readonly ApiClient _api;

    public InsuranceService(ApiClient api) => _api = api;

    public Task<IReadOnlyList<InsuranceCompanyDto>> GetCompaniesAsync(CancellationToken ct = default)
        => _api.GetAsync<IReadOnlyList<InsuranceCompanyDto>>("api/insurance/companies", ct);

    public Task<InsuranceCompanyDto> GetCompanyByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<InsuranceCompanyDto>($"api/insurance/companies/{id}", ct);

    public Task<InsuranceCompanyDto> CreateCompanyAsync(InsuranceCompanyInput input, CancellationToken ct = default)
        => _api.PostAsync<InsuranceCompanyDto>("api/insurance/companies", input, ct);

    public Task UpdateCompanyAsync(string id, InsuranceCompanyInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/insurance/companies/{id}", input, ct);

    public Task DeleteCompanyAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/insurance/companies/{id}", ct);

    public Task<PagedResult<InsurancePolicyListItemDto>> GetPoliciesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

        return _api.PostAsync<PagedResult<InsurancePolicyListItemDto>>("api/insurance/policies/all", query, ct);
    }

    public Task<InsurancePolicyDto> GetPolicyByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<InsurancePolicyDto>($"api/insurance/policies/{id}", ct);

    public Task<IReadOnlyList<InsurancePolicyDto>> GetPoliciesByPatientAsync(string patientId, CancellationToken ct = default)
        => _api.GetAsync<IReadOnlyList<InsurancePolicyDto>>($"api/insurance/policies/by-patient/{patientId}", ct);

    public Task<InsurancePolicyDto> CreatePolicyAsync(InsurancePolicyInput input, CancellationToken ct = default)
        => _api.PostAsync<InsurancePolicyDto>("api/insurance/policies", input, ct);

    public Task<InsurancePolicyDto> UpdateCoverageAsync(string id, UpdatePolicyCoverageInput input, CancellationToken ct = default)
        => _api.PutAsync<InsurancePolicyDto>($"api/insurance/policies/{id}/coverage", input, ct);

    public Task<InsurancePolicyDto> ChangePolicyStatusAsync(string id, string action, CancellationToken ct = default)
        => _api.PostAsync<InsurancePolicyDto>($"api/insurance/policies/{id}/status", new ChangePolicyStatusInput { Action = action }, ct);

    public Task<IReadOnlyList<PolicyDocumentDto>> GetPolicyDocumentsAsync(string policyId, CancellationToken ct = default)
        => _api.GetAsync<IReadOnlyList<PolicyDocumentDto>>($"api/insurance/policies/{policyId}/documents", ct);

    public Task<PolicyDocumentDto> UploadPolicyDocumentAsync(
        string policyId,
        string type,
        string fileName,
        string contentType,
        Stream stream,
        CancellationToken ct = default)
    {
        var fields = new Dictionary<string, string> { ["type"] = type };
        return _api.PostFormAsync<PolicyDocumentDto>(
            $"api/insurance/policies/{policyId}/documents",
            fields,
            ("file", fileName, contentType, stream),
            ct);
    }

    public Task<byte[]> DownloadPolicyDocumentAsync(string policyId, string documentId, CancellationToken ct = default)
        => _api.GetBytesAsync($"api/insurance/policies/{policyId}/documents/{documentId}/content", ct);

    public Task DeletePolicyDocumentAsync(string policyId, string documentId, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/insurance/policies/{policyId}/documents/{documentId}", ct);

    public Task<PagedResult<PreAuthorizationListItemDto>> GetPreAuthorizationsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

        return _api.PostAsync<PagedResult<PreAuthorizationListItemDto>>("api/insurance/preauthorizations/all", query, ct);
    }

    public Task<PreAuthorizationDto> GetPreAuthorizationByExaminationAsync(string examinationId, CancellationToken ct = default)
        => _api.GetAsync<PreAuthorizationDto>($"api/insurance/preauthorizations/by-examination/{examinationId}", ct);

    public Task<PreAuthorizationDto> CreatePreAuthorizationAsync(CreatePreAuthorizationInput input, CancellationToken ct = default)
        => _api.PostAsync<PreAuthorizationDto>("api/insurance/preauthorizations", input, ct);

    public Task<PreAuthorizationDto> DecidePreAuthorizationAsync(string id, DecidePreAuthorizationInput input, CancellationToken ct = default)
        => _api.PostAsync<PreAuthorizationDto>($"api/insurance/preauthorizations/{id}/decide", input, ct);

    public Task<IReadOnlyList<PreAuthorizationDocumentDto>> GetPreAuthorizationDocumentsAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<IReadOnlyList<PreAuthorizationDocumentDto>>($"api/insurance/preauthorizations/{id}/documents", ct);

    public Task<PreAuthorizationDocumentDto> UploadPreAuthorizationDocumentAsync(
        string id,
        string type,
        string fileName,
        string contentType,
        Stream stream,
        CancellationToken ct = default)
    {
        var fields = new Dictionary<string, string> { ["type"] = type };
        return _api.PostFormAsync<PreAuthorizationDocumentDto>(
            $"api/insurance/preauthorizations/{id}/documents",
            fields,
            ("file", fileName, contentType, stream),
            ct);
    }

    public Task<byte[]> DownloadPreAuthorizationDocumentAsync(string id, string documentId, CancellationToken ct = default)
        => _api.GetBytesAsync($"api/insurance/preauthorizations/{id}/documents/{documentId}/content", ct);

    public Task DeletePreAuthorizationDocumentAsync(string id, string documentId, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/insurance/preauthorizations/{id}/documents/{documentId}", ct);

    public Task<PagedResult<ClaimListItemDto>> GetClaimsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

        return _api.PostAsync<PagedResult<ClaimListItemDto>>("api/insurance/claims/all", query, ct);
    }

    public Task<ClaimDto> GetClaimByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<ClaimDto>($"api/insurance/claims/{id}", ct);

    public Task<ClaimDto> GetClaimByExaminationAsync(string examinationId, CancellationToken ct = default)
        => _api.GetAsync<ClaimDto>($"api/insurance/claims/by-examination/{examinationId}", ct);

    public Task<ClaimDto> CreateClaimAsync(CreateClaimInput input, CancellationToken ct = default)
        => _api.PostAsync<ClaimDto>("api/insurance/claims", input, ct);

    public Task<ClaimDto> SubmitClaimAsync(string id, CancellationToken ct = default)
        => _api.PostAsync<ClaimDto>($"api/insurance/claims/{id}/submit", null, ct);

    public Task<ClaimDto> AdjudicateClaimAsync(string id, AdjudicateClaimInput input, CancellationToken ct = default)
        => _api.PostAsync<ClaimDto>($"api/insurance/claims/{id}/adjudicate", input, ct);

    public Task<ClaimDto> ResubmitClaimAsync(string id, CancellationToken ct = default)
        => _api.PostAsync<ClaimDto>($"api/insurance/claims/{id}/resubmit", null, ct);

    public Task<ClaimDto> RecordSettlementAsync(string id, RecordSettlementInput input, CancellationToken ct = default)
        => _api.PostAsync<ClaimDto>($"api/insurance/claims/{id}/settlements", input, ct);

    public Task<InsuranceStatsDto> GetStatsAsync(CancellationToken ct = default)
        => _api.GetAsync<InsuranceStatsDto>("api/insurance/stats", ct);
}
