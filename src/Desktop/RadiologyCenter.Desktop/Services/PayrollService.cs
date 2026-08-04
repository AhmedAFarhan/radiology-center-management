using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class PayrollService
{
    private readonly ApiClient _api;

    public PayrollService(ApiClient api) => _api = api;

    private static object BuildQuery(string? searchTerm, string? sortBy, bool sortDescending, int pageNumber, int pageSize)
        => new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

    // ----- Pay Runs -----
    public Task<PagedResult<PayRunDto>> GetPayRunsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => _api.PostAsync<PagedResult<PayRunDto>>("api/payroll/payruns/all", BuildQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    public Task<PayRunDto> GetPayRunByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<PayRunDto>($"api/payroll/payruns/{id}", ct);

    public Task<PayRunDto> CreatePayRunAsync(CreatePayRunInput input, CancellationToken ct = default)
        => _api.PostAsync<PayRunDto>("api/payroll/payruns", input, ct);

    public Task<PayslipDto> AddPayslipAsync(string payRunId, string staffId, CancellationToken ct = default)
        => _api.PostAsync<PayslipDto>($"api/payroll/payruns/{payRunId}/payslips", new { staffId }, ct);

    public Task RemovePayslipAsync(string payRunId, string staffId, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/payroll/payruns/{payRunId}/payslips/{staffId}", ct);

    public Task ComputePayRunAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/payruns/{id}/compute", ct: ct);

    public Task ApprovePayRunAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/payruns/{id}/approve", ct: ct);

    public Task RejectPayRunAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/payruns/{id}/reject", ct: ct);

    public Task RestartPayRunAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/payruns/{id}/restart", ct: ct);

    public Task PayPayRunAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/payruns/{id}/pay", ct: ct);

    public Task DeletePayRunAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/payroll/payruns/{id}", ct);

    // ----- Salary Components -----
    public Task<PagedResult<SalaryComponentDto>> GetSalaryComponentsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => _api.PostAsync<PagedResult<SalaryComponentDto>>("api/payroll/salary-components/all", BuildQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    public Task<SalaryComponentDto> GetSalaryComponentByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<SalaryComponentDto>($"api/payroll/salary-components/{id}", ct);

    public Task<SalaryComponentDto> CreateSalaryComponentAsync(SalaryComponentInput input, CancellationToken ct = default)
        => _api.PostAsync<SalaryComponentDto>("api/payroll/salary-components", input, ct);

    public Task UpdateSalaryComponentAsync(string id, SalaryComponentInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/payroll/salary-components/{id}", input, ct);

    public Task ActivateSalaryComponentAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/salary-components/{id}/activate", ct: ct);

    public Task DeactivateSalaryComponentAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/salary-components/{id}/deactivate", ct: ct);

    public Task DeleteSalaryComponentAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/payroll/salary-components/{id}", ct);

    // ----- Salaries -----
    public Task<PagedResult<SalaryDto>> GetSalariesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => _api.PostAsync<PagedResult<SalaryDto>>("api/payroll/salaries/all", BuildQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    public Task<SalaryDto> GetSalaryByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<SalaryDto>($"api/payroll/salaries/{id}", ct);

    public Task<SalaryDto> CreateSalaryAsync(SalaryInput input, CancellationToken ct = default)
        => _api.PostAsync<SalaryDto>("api/payroll/salaries", input, ct);

    public Task UpdateSalaryAsync(string id, SalaryInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/payroll/salaries/{id}", input, ct);

    public Task ActivateSalaryAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/salaries/{id}/activate", ct: ct);

    public Task DeactivateSalaryAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/salaries/{id}/deactivate", ct: ct);

    public Task DeleteSalaryAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/payroll/salaries/{id}", ct);

    // ----- Allowances -----
    public Task<PagedResult<AllowanceAssignmentDto>> GetAllowancesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => _api.PostAsync<PagedResult<AllowanceAssignmentDto>>("api/payroll/allowances/all", BuildQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    public Task<AllowanceAssignmentDto> GetAllowanceByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<AllowanceAssignmentDto>($"api/payroll/allowances/{id}", ct);

    public Task<AllowanceAssignmentDto> CreateAllowanceAsync(AllowanceAssignmentInput input, CancellationToken ct = default)
        => _api.PostAsync<AllowanceAssignmentDto>("api/payroll/allowances", input, ct);

    public Task UpdateAllowanceAsync(string id, AllowanceAssignmentInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/payroll/allowances/{id}", input, ct);

    public Task ActivateAllowanceAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/allowances/{id}/activate", ct: ct);

    public Task DeactivateAllowanceAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/allowances/{id}/deactivate", ct: ct);

    public Task DeleteAllowanceAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/payroll/allowances/{id}", ct);

    // ----- Examination Fees -----
    public Task<PagedResult<ExaminationFeeDto>> GetExaminationFeesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => _api.PostAsync<PagedResult<ExaminationFeeDto>>("api/payroll/examination-fees/all", BuildQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    public Task<ExaminationFeeDto> GetExaminationFeeByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<ExaminationFeeDto>($"api/payroll/examination-fees/{id}", ct);

    public Task<ExaminationFeeDto> CreateExaminationFeeAsync(ExaminationFeeInput input, CancellationToken ct = default)
        => _api.PostAsync<ExaminationFeeDto>("api/payroll/examination-fees", input, ct);

    public Task UpdateExaminationFeeAsync(string id, ExaminationFeeInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/payroll/examination-fees/{id}", input, ct);

    public Task ActivateExaminationFeeAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/examination-fees/{id}/activate", ct: ct);

    public Task DeactivateExaminationFeeAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/examination-fees/{id}/deactivate", ct: ct);

    public Task DeleteExaminationFeeAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/payroll/examination-fees/{id}", ct);

    // ----- Referral Fees -----
    public Task<PagedResult<ReferralFeeDto>> GetReferralFeesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => _api.PostAsync<PagedResult<ReferralFeeDto>>("api/payroll/referral-fees/all", BuildQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    public Task<ReferralFeeDto> GetReferralFeeByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<ReferralFeeDto>($"api/payroll/referral-fees/{id}", ct);

    public Task<ReferralFeeDto> CreateReferralFeeAsync(ReferralFeeInput input, CancellationToken ct = default)
        => _api.PostAsync<ReferralFeeDto>("api/payroll/referral-fees", input, ct);

    public Task UpdateReferralFeeAsync(string id, ReferralFeeInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/payroll/referral-fees/{id}", input, ct);

    public Task ActivateReferralFeeAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/referral-fees/{id}/activate", ct: ct);

    public Task DeactivateReferralFeeAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/payroll/referral-fees/{id}/deactivate", ct: ct);

    public Task DeleteReferralFeeAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/payroll/referral-fees/{id}", ct);
}