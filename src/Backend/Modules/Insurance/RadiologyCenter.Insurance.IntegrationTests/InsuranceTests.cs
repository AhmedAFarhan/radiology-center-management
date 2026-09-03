using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class InsuranceTests : TestBase
{
    private const string CompaniesUrl = "api/insurance/companies";
    private const string PoliciesUrl = "api/insurance/policies";
    private const string ClaimsUrl = "api/insurance/claims";
    private const string PreAuthUrl = "api/insurance/preauthorizations";
    private const string StatsUrl = "api/insurance/stats";

    public InsuranceTests(CustomWebApplicationFactory factory) : base(factory) { }

    // ── Insurance Companies ──────────────────────────────────────────────

    [Fact]
    public async Task CreateCompany_ValidData_ReturnsOk()
    {
        var command = new { Name = $"Company_{Guid.NewGuid():N}", TaxId = "TAX123", Address = "123 Main St", Phone = "0123456789", Email = "test@company.com" };
        var response = await Client.PostAsJsonAsync(CompaniesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateCompany_MissingName_ReturnsBadRequest()
    {
        var command = new { Name = "", Phone = "0123456789" };
        var response = await Client.PostAsJsonAsync(CompaniesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCompany_InvalidEmail_ReturnsBadRequest()
    {
        var command = new { Name = $"Company_{Guid.NewGuid():N}", Email = "not-an-email" };
        var response = await Client.PostAsJsonAsync(CompaniesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCompanyById_Existing_ReturnsOk()
    {
        var id = await CreateTestCompanyAsync();
        var response = await Client.GetAsync($"{CompaniesUrl}/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CompanyDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsOk()
    {
        var response = await Client.GetAsync(CompaniesUrl);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateCompany_ValidData_ReturnsOk()
    {
        var id = await CreateTestCompanyAsync();
        var command = new { Name = $"Updated_{Guid.NewGuid():N}", TaxId = "TAX456", Address = "456 Updated St", Phone = "9876543210", Email = "updated@company.com" };
        var response = await Client.PutAsJsonAsync($"{CompaniesUrl}/{id}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateCompany_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { Name = $"Ghost_{Guid.NewGuid():N}", TaxId = "TAX000", Address = "Ghost St", Phone = "0000000000", Email = "ghost@company.com" };
        var response = await Client.PutAsJsonAsync($"{CompaniesUrl}/{fakeId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCompany_ReturnsOk()
    {
        var id = await CreateTestCompanyAsync();
        var response = await Client.DeleteAsync($"{CompaniesUrl}/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteCompany_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{CompaniesUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateDuplicateCompanyName_ReturnsConflict()
    {
        var name = $"DupCompany_{Guid.NewGuid():N}";
        await Client.PostAsJsonAsync(CompaniesUrl, new { Name = name });
        var response = await Client.PostAsJsonAsync(CompaniesUrl, new { Name = name });
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    // ── Insurance Policies ───────────────────────────────────────────────

    [Fact]
    public async Task CreatePolicy_ValidData_ReturnsOk()
    {
        var companyId = await CreateTestCompanyAsync();
        var patientId = Guid.NewGuid();
        var command = new { CompanyId = companyId, PatientId = patientId, PolicyNumber = $"POL-{Guid.NewGuid():N}", CoveragePercent = 75m, EffectiveFrom = DateTime.UtcNow, IsGovernment = false };
        var response = await Client.PostAsJsonAsync(PoliciesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreatePolicy_MissingCompanyId_ReturnsBadRequest()
    {
        var command = new { CompanyId = Guid.Empty, PatientId = Guid.NewGuid(), PolicyNumber = $"POL-{Guid.NewGuid():N}", CoveragePercent = 75m, EffectiveFrom = DateTime.UtcNow, IsGovernment = false };
        var response = await Client.PostAsJsonAsync(PoliciesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePolicy_MissingPolicyNumber_ReturnsBadRequest()
    {
        var companyId = await CreateTestCompanyAsync();
        var command = new { CompanyId = companyId, PatientId = Guid.NewGuid(), PolicyNumber = "", CoveragePercent = 75m, EffectiveFrom = DateTime.UtcNow, IsGovernment = false };
        var response = await Client.PostAsJsonAsync(PoliciesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePolicy_CoveragePercentOutOfRange_ReturnsBadRequest()
    {
        var companyId = await CreateTestCompanyAsync();
        var command = new { CompanyId = companyId, PatientId = Guid.NewGuid(), PolicyNumber = $"POL-{Guid.NewGuid():N}", CoveragePercent = 150m, EffectiveFrom = DateTime.UtcNow, IsGovernment = false };
        var response = await Client.PostAsJsonAsync(PoliciesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePolicy_MissingEffectiveFrom_ReturnsBadRequest()
    {
        var companyId = await CreateTestCompanyAsync();
        var command = new { CompanyId = companyId, PatientId = Guid.NewGuid(), PolicyNumber = $"POL-{Guid.NewGuid():N}", CoveragePercent = 75m, EffectiveFrom = (DateTime?)null, IsGovernment = false };
        var response = await Client.PostAsJsonAsync(PoliciesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPolicyById_Existing_ReturnsOk()
    {
        var policyId = await CreateTestPolicyAsync();
        var response = await Client.GetAsync($"{PoliciesUrl}/{policyId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PolicyDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(policyId);
    }

    [Fact]
    public async Task GetPoliciesByPatient_ReturnsOk()
    {
        var companyId = await CreateTestCompanyAsync();
        var patientId = Guid.NewGuid();
        await CreateTestPolicyForPatientAsync(companyId, patientId);
        var response = await Client.GetAsync($"{PoliciesUrl}/by-patient/{patientId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdatePolicyCoverage_ReturnsOk()
    {
        var policyId = await CreateTestPolicyAsync();
        var command = new { PolicyId = policyId, CoveragePercent = 90m };
        var response = await Client.PutAsJsonAsync($"{PoliciesUrl}/{policyId}/coverage", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangePolicyStatus_ReturnsOk()
    {
        var policyId = await CreateTestPolicyAsync();
        var command = new { PolicyId = policyId, Action = "Activate" };
        var response = await Client.PostAsJsonAsync($"{PoliciesUrl}/{policyId}/status", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── Claims ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateClaim_ValidData_ReturnsOk()
    {
        var (policyId, patientId, examinationId, preAuthId) = await CreateClaimPrerequisitesAsync();
        var command = new { ExaminationId = examinationId, PatientId = patientId, PolicyId = policyId, PreAuthorizationId = preAuthId, BilledAmount = 500m };
        var response = await Client.PostAsJsonAsync(ClaimsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateClaim_MissingExaminationId_ReturnsBadRequest()
    {
        var (policyId, patientId, _, preAuthId) = await CreateClaimPrerequisitesAsync();
        var command = new { ExaminationId = Guid.Empty, PatientId = patientId, PolicyId = policyId, PreAuthorizationId = preAuthId, BilledAmount = 500m };
        var response = await Client.PostAsJsonAsync(ClaimsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateClaim_NegativeBilledAmount_ReturnsBadRequest()
    {
        var (policyId, patientId, examinationId, preAuthId) = await CreateClaimPrerequisitesAsync();
        var command = new { ExaminationId = examinationId, PatientId = patientId, PolicyId = policyId, PreAuthorizationId = preAuthId, BilledAmount = -100m };
        var response = await Client.PostAsJsonAsync(ClaimsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetClaimById_Existing_ReturnsOk()
    {
        var claimId = await CreateTestClaimAsync();
        var response = await Client.GetAsync($"{ClaimsUrl}/{claimId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ClaimDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(claimId);
    }

    [Fact]
    public async Task GetClaimByExamination_ReturnsOk()
    {
        var (_, _, examinationId, _) = await CreateClaimPrerequisitesAsync();
        await CreateTestClaimForExaminationAsync(examinationId);
        var response = await Client.GetAsync($"{ClaimsUrl}/by-examination/{examinationId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SubmitClaim_ReturnsOk()
    {
        var claimId = await CreateTestClaimAsync();
        var response = await Client.PostAsJsonAsync($"{ClaimsUrl}/{claimId}/submit", new { ClaimId = claimId });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdjudicateClaim_Approve_ReturnsOk()
    {
        var claimId = await CreateTestClaimAsync();
        await Client.PostAsJsonAsync($"{ClaimsUrl}/{claimId}/submit", new { ClaimId = claimId });
        var command = new { ClaimId = claimId, Decision = "Approve", ApprovedAmount = 400m };
        var response = await Client.PostAsJsonAsync($"{ClaimsUrl}/{claimId}/adjudicate", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdjudicateClaim_Reject_ReturnsOk()
    {
        var claimId = await CreateTestClaimAsync();
        await Client.PostAsJsonAsync($"{ClaimsUrl}/{claimId}/submit", new { ClaimId = claimId });
        var command = new { ClaimId = claimId, Decision = "Reject", RejectionCode = "C01", RejectionReason = "Not covered" };
        var response = await Client.PostAsJsonAsync($"{ClaimsUrl}/{claimId}/adjudicate", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResubmitClaim_ReturnsOk()
    {
        var claimId = await CreateTestClaimAsync();
        await Client.PostAsJsonAsync($"{ClaimsUrl}/{claimId}/submit", new { ClaimId = claimId });
        var command = new { ClaimId = claimId };
        var response = await Client.PostAsJsonAsync($"{ClaimsUrl}/{claimId}/resubmit", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RecordClaimSettlement_ReturnsOk()
    {
        var claimId = await CreateTestClaimAsync();
        var command = new { ClaimId = claimId, Method = "BankTransfer", Amount = 350m };
        var response = await Client.PostAsJsonAsync($"{ClaimsUrl}/{claimId}/settlements", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── Pre-Authorizations ───────────────────────────────────────────────

    [Fact]
    public async Task CreatePreAuthorization_ValidData_ReturnsOk()
    {
        var companyId = await CreateTestCompanyAsync();
        var patientId = Guid.NewGuid();
        var policyId = await CreateTestPolicyForPatientAsync(companyId, patientId);
        var examinationId = Guid.NewGuid();
        var command = new { ExaminationId = examinationId, PatientId = patientId, PolicyId = policyId, EstimatedAmount = 1000m };
        var response = await Client.PostAsJsonAsync(PreAuthUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreatePreAuthorization_MissingExaminationId_ReturnsBadRequest()
    {
        var companyId = await CreateTestCompanyAsync();
        var patientId = Guid.NewGuid();
        var policyId = await CreateTestPolicyForPatientAsync(companyId, patientId);
        var command = new { ExaminationId = Guid.Empty, PatientId = patientId, PolicyId = policyId, EstimatedAmount = 1000m };
        var response = await Client.PostAsJsonAsync(PreAuthUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePreAuthorization_NegativeEstimatedAmount_ReturnsBadRequest()
    {
        var companyId = await CreateTestCompanyAsync();
        var patientId = Guid.NewGuid();
        var policyId = await CreateTestPolicyForPatientAsync(companyId, patientId);
        var examinationId = Guid.NewGuid();
        var command = new { ExaminationId = examinationId, PatientId = patientId, PolicyId = policyId, EstimatedAmount = -500m };
        var response = await Client.PostAsJsonAsync(PreAuthUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DecidePreAuthorization_Approve_ReturnsOk()
    {
        var (preAuthId, _) = await CreateTestPreAuthorizationAsync();
        var command = new { PreAuthorizationId = preAuthId, Decision = "Approve", ApprovedAmount = 800m };
        var response = await Client.PostAsJsonAsync($"{PreAuthUrl}/{preAuthId}/decide", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DecidePreAuthorization_DenyWithoutReason_ReturnsBadRequest()
    {
        var (preAuthId, _) = await CreateTestPreAuthorizationAsync();
        var command = new { PreAuthorizationId = preAuthId, Decision = "Deny", RejectionReason = (string?)null };
        var response = await Client.PostAsJsonAsync($"{PreAuthUrl}/{preAuthId}/decide", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPreAuthorizationByExamination_ReturnsOk()
    {
        var (_, examinationId) = await CreateTestPreAuthorizationAsync();
        var response = await Client.GetAsync($"{PreAuthUrl}/by-examination/{examinationId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── Stats ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetStats_ReturnsOk()
    {
        var response = await Client.GetAsync(StatsUrl);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    private async Task<Guid> CreateTestCompanyAsync()
    {
        var command = new { Name = $"TestCompany_{Guid.NewGuid():N}", TaxId = $"TAX-{Guid.NewGuid():N}", Address = "Test Address", Phone = "0123456789", Email = $"test_{Guid.NewGuid():N}@company.com" };
        var response = await Client.PostAsJsonAsync(CompaniesUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"CreateTestCompanyAsync failed: expected OK but got {response.StatusCode}");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
        if (body == null || !body.Success || body.Data == Guid.Empty)
            throw new Exception($"CreateTestCompanyAsync returned unsuccessful response: {body?.Message ?? "null"}");
        return body.Data;
    }

    private async Task<Guid> CreateTestPolicyForPatientAsync(Guid companyId, Guid patientId)
    {
        var command = new { CompanyId = companyId, PatientId = patientId, PolicyNumber = $"POL-{Guid.NewGuid():N}", CoveragePercent = 75m, EffectiveFrom = DateTime.UtcNow, IsGovernment = false };
        var response = await Client.PostAsJsonAsync(PoliciesUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"CreateTestPolicyForPatientAsync failed: expected OK but got {response.StatusCode}");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
        if (body == null || !body.Success || body.Data == Guid.Empty)
            throw new Exception($"CreateTestPolicyForPatientAsync returned unsuccessful response: {body?.Message ?? "null"}");
        return body.Data;
    }

    private async Task<Guid> CreateTestPolicyAsync()
    {
        var companyId = await CreateTestCompanyAsync();
        var patientId = Guid.NewGuid();
        return await CreateTestPolicyForPatientAsync(companyId, patientId);
    }

    private async Task<(Guid PolicyId, Guid PatientId, Guid ExaminationId, Guid PreAuthId)> CreateClaimPrerequisitesAsync()
    {
        var companyId = await CreateTestCompanyAsync();
        var patientId = Guid.NewGuid();
        var policyId = await CreateTestPolicyForPatientAsync(companyId, patientId);
        var examinationId = Guid.NewGuid();
        var preAuthCommand = new { ExaminationId = examinationId, PatientId = patientId, PolicyId = policyId, EstimatedAmount = 1000m };
        var preAuthResponse = await Client.PostAsJsonAsync(PreAuthUrl, preAuthCommand);
        if (preAuthResponse.StatusCode != HttpStatusCode.OK)
            throw new Exception($"CreateClaimPrerequisitesAsync (pre-auth) failed: expected OK but got {preAuthResponse.StatusCode}");
        var preAuthBody = await preAuthResponse.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
        if (preAuthBody == null || !preAuthBody.Success || preAuthBody.Data == Guid.Empty)
            throw new Exception($"CreateClaimPrerequisitesAsync (pre-auth) returned unsuccessful response: {preAuthBody?.Message ?? "null"}");
        var preAuthId = preAuthBody.Data;
        return (policyId, patientId, examinationId, preAuthId);
    }

    private async Task<Guid> CreateTestClaimAsync()
    {
        var (policyId, patientId, examinationId, preAuthId) = await CreateClaimPrerequisitesAsync();
        return await CreateTestClaimForExaminationAsync(examinationId, patientId, policyId, preAuthId);
    }

    private async Task<Guid> CreateTestClaimForExaminationAsync(Guid examinationId, Guid? patientId = null, Guid? policyId = null, Guid? preAuthId = null)
    {
        if (!patientId.HasValue || !policyId.HasValue || !preAuthId.HasValue)
        {
            var prereq = await CreateClaimPrerequisitesAsync();
            patientId ??= prereq.PatientId;
            policyId ??= prereq.PolicyId;
            preAuthId ??= prereq.PreAuthId;
        }
        var command = new { ExaminationId = examinationId, PatientId = patientId.Value, PolicyId = policyId.Value, PreAuthorizationId = preAuthId.Value, BilledAmount = 500m };
        var response = await Client.PostAsJsonAsync(ClaimsUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"CreateTestClaimForExaminationAsync failed: expected OK but got {response.StatusCode}");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
        if (body == null || !body.Success || body.Data == Guid.Empty)
            throw new Exception($"CreateTestClaimForExaminationAsync returned unsuccessful response: {body?.Message ?? "null"}");
        return body.Data;
    }

    private async Task<(Guid PreAuthId, Guid ExaminationId)> CreateTestPreAuthorizationAsync()
    {
        var companyId = await CreateTestCompanyAsync();
        var patientId = Guid.NewGuid();
        var policyId = await CreateTestPolicyForPatientAsync(companyId, patientId);
        var examinationId = Guid.NewGuid();
        var command = new { ExaminationId = examinationId, PatientId = patientId, PolicyId = policyId, EstimatedAmount = 1000m };
        var response = await Client.PostAsJsonAsync(PreAuthUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"CreateTestPreAuthorizationAsync failed: expected OK but got {response.StatusCode}");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
        if (body == null || !body.Success || body.Data == Guid.Empty)
            throw new Exception($"CreateTestPreAuthorizationAsync returned unsuccessful response: {body?.Message ?? "null"}");
        return (body.Data, examinationId);
    }

    // ── DTOs ─────────────────────────────────────────────────────────────

    private sealed class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public ApiErrorDto? Error { get; set; }
    }

    private sealed class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public ApiErrorDto? Error { get; set; }
    }

    private sealed class ApiErrorDto
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }

    private sealed class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    private sealed class PolicyDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid PatientId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public decimal CoveragePercent { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsGovernment { get; set; }
    }

    private sealed class ClaimDto
    {
        public Guid Id { get; set; }
        public Guid ExaminationId { get; set; }
        public Guid PatientId { get; set; }
        public Guid PolicyId { get; set; }
        public decimal BilledAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    private sealed class PreAuthorizationDto
    {
        public Guid Id { get; set; }
        public Guid ExaminationId { get; set; }
        public Guid PatientId { get; set; }
        public Guid PolicyId { get; set; }
        public decimal EstimatedAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
