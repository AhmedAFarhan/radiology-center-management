using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Payroll.Services;

public sealed class PayrollService : CrudServiceBase
{
    private const string PayRunsRes = "api/payroll/payruns";
    private const string SalaryComponentsRes = "api/payroll/salary-components";
    private const string SalariesRes = "api/payroll/salaries";
    private const string AllowancesRes = "api/payroll/allowances";
    private const string ExaminationFeesRes = "api/payroll/examination-fees";
    private const string ReferralFeesRes = "api/payroll/referral-fees";

    public PayrollService(ApiClient api) : base(api) { }

    // ----- Pay Runs -----
    public Task<PagedResult<PayRunListItemDto>> GetPayRunsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<PayRunListItemDto>(PayRunsRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<PayRunDto> GetPayRunByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<PayRunDto>(PayRunsRes, id, ct);

    public Task<PayRunDto> CreatePayRunAsync(CreatePayRunInput input, CancellationToken ct = default)
        => CreateEntityAsync<PayRunDto>(PayRunsRes, input, ct);

    public Task<PayslipDto> AddPayslipAsync(string payRunId, string staffId, CancellationToken ct = default)
        => Api.PostAsync<PayslipDto>($"{PayRunsRes}/{payRunId}/payslips", new { staffId }, ct);

    public Task RemovePayslipAsync(string payRunId, string staffId, CancellationToken ct = default)
        => Api.SendDeleteAsync($"{PayRunsRes}/{payRunId}/payslips/{staffId}", ct);

    public Task ComputePayRunAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{PayRunsRes}/{id}/compute", ct: ct);

    public Task ApprovePayRunAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{PayRunsRes}/{id}/approve", ct: ct);

    public Task RejectPayRunAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{PayRunsRes}/{id}/reject", ct: ct);

    public Task RestartPayRunAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{PayRunsRes}/{id}/restart", ct: ct);

    public Task PayPayRunAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{PayRunsRes}/{id}/pay", ct: ct);

    public Task DeletePayRunAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(PayRunsRes, id, ct);

    public Task<byte[]> GetPayslipPdfAsync(string payRunId, string staffId, CancellationToken ct = default)
        => Api.GetBytesAsync($"{PayRunsRes}/{payRunId}/payslips/{staffId}/pdf", ct);

    public Task<byte[]> GetReferralStatementPdfAsync(string payRunId, string doctorId, CancellationToken ct = default)
        => Api.GetBytesAsync($"{PayRunsRes}/{payRunId}/referral-statements/{doctorId}/pdf", ct);

    // ----- Salary Components -----
    public Task<PagedResult<SalaryComponentDto>> GetSalaryComponentsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<SalaryComponentDto>(SalaryComponentsRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<SalaryComponentDto> GetSalaryComponentByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<SalaryComponentDto>(SalaryComponentsRes, id, ct);

    public Task<SalaryComponentDto> CreateSalaryComponentAsync(SalaryComponentInput input, CancellationToken ct = default)
        => CreateEntityAsync<SalaryComponentDto>(SalaryComponentsRes, input, ct);

    public Task UpdateSalaryComponentAsync(string id, SalaryComponentInput input, CancellationToken ct = default)
        => UpdateEntityAsync(SalaryComponentsRes, id, input, ct);

    public Task ActivateSalaryComponentAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(SalaryComponentsRes, id, true, ct);

    public Task DeactivateSalaryComponentAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(SalaryComponentsRes, id, false, ct);

    public Task DeleteSalaryComponentAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(SalaryComponentsRes, id, ct);

    // ----- Salaries -----
    public Task<PagedResult<SalaryDto>> GetSalariesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<SalaryDto>(SalariesRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<SalaryDto> GetSalaryByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<SalaryDto>(SalariesRes, id, ct);

    public Task<SalaryDto> CreateSalaryAsync(SalaryInput input, CancellationToken ct = default)
        => CreateEntityAsync<SalaryDto>(SalariesRes, input, ct);

    public Task UpdateSalaryAsync(string id, SalaryInput input, CancellationToken ct = default)
        => UpdateEntityAsync(SalariesRes, id, input, ct);

    public Task ActivateSalaryAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(SalariesRes, id, true, ct);

    public Task DeactivateSalaryAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(SalariesRes, id, false, ct);

    public Task DeleteSalaryAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(SalariesRes, id, ct);

    // ----- Allowances -----
    public Task<PagedResult<AllowanceAssignmentDto>> GetAllowancesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<AllowanceAssignmentDto>(AllowancesRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<AllowanceAssignmentDto> GetAllowanceByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<AllowanceAssignmentDto>(AllowancesRes, id, ct);

    public Task<AllowanceAssignmentDto> CreateAllowanceAsync(AllowanceAssignmentInput input, CancellationToken ct = default)
        => CreateEntityAsync<AllowanceAssignmentDto>(AllowancesRes, input, ct);

    public Task UpdateAllowanceAsync(string id, AllowanceAssignmentInput input, CancellationToken ct = default)
        => UpdateEntityAsync(AllowancesRes, id, input, ct);

    public Task ActivateAllowanceAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(AllowancesRes, id, true, ct);

    public Task DeactivateAllowanceAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(AllowancesRes, id, false, ct);

    public Task DeleteAllowanceAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(AllowancesRes, id, ct);

    // ----- Examination Fees -----
    public Task<PagedResult<ExaminationFeeDto>> GetExaminationFeesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<ExaminationFeeDto>(ExaminationFeesRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<ExaminationFeeDto> GetExaminationFeeByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<ExaminationFeeDto>(ExaminationFeesRes, id, ct);

    public Task<ExaminationFeeDto> CreateExaminationFeeAsync(ExaminationFeeInput input, CancellationToken ct = default)
        => CreateEntityAsync<ExaminationFeeDto>(ExaminationFeesRes, input, ct);

    public Task UpdateExaminationFeeAsync(string id, ExaminationFeeInput input, CancellationToken ct = default)
        => UpdateEntityAsync(ExaminationFeesRes, id, input, ct);

    public Task ActivateExaminationFeeAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(ExaminationFeesRes, id, true, ct);

    public Task DeactivateExaminationFeeAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(ExaminationFeesRes, id, false, ct);

    public Task DeleteExaminationFeeAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(ExaminationFeesRes, id, ct);

    // ----- Referral Fees -----
    public Task<PagedResult<ReferralFeeDto>> GetReferralFeesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<ReferralFeeDto>(ReferralFeesRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<ReferralFeeDto> GetReferralFeeByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<ReferralFeeDto>(ReferralFeesRes, id, ct);

    public Task<ReferralFeeDto> CreateReferralFeeAsync(ReferralFeeInput input, CancellationToken ct = default)
        => CreateEntityAsync<ReferralFeeDto>(ReferralFeesRes, input, ct);

    public Task UpdateReferralFeeAsync(string id, ReferralFeeInput input, CancellationToken ct = default)
        => UpdateEntityAsync(ReferralFeesRes, id, input, ct);

    public Task ActivateReferralFeeAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(ReferralFeesRes, id, true, ct);

    public Task DeactivateReferralFeeAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(ReferralFeesRes, id, false, ct);

    public Task DeleteReferralFeeAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(ReferralFeesRes, id, ct);
}
