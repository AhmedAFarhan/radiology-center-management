using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Services;

public class PayrollPayslipCalculator : IPayslipCalculator
{
    private const int WorkingDaysPerMonth = 26;

    private readonly ISalaryRepository _salaryRepository;
    private readonly IAllowanceAssignmentRepository _allowanceAssignmentRepository;
    private readonly ISalaryComponentRepository _salaryComponentRepository;
    private readonly IExamFeeIncomeResolver _examFeeIncomeResolver;
    private readonly IStaffLeaveResolver _staffLeaveResolver;

    public PayrollPayslipCalculator(
        ISalaryRepository salaryRepository,
        IAllowanceAssignmentRepository allowanceAssignmentRepository,
        ISalaryComponentRepository salaryComponentRepository,
        IExamFeeIncomeResolver examFeeIncomeResolver,
        IStaffLeaveResolver staffLeaveResolver)
    {
        _salaryRepository = salaryRepository;
        _allowanceAssignmentRepository = allowanceAssignmentRepository;
        _salaryComponentRepository = salaryComponentRepository;
        _examFeeIncomeResolver = examFeeIncomeResolver;
        _staffLeaveResolver = staffLeaveResolver;
    }

    public async Task<PayrollPayslipDraft?> CalculateAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct)
    {
        var salary = await FindBaseSalaryAsync(staffId, to, ct);
        var baseSalary = salary?.BaseSalary ?? 0m;

        var allowances = await FindActiveAllowancesAsync(staffId, from, to, ct);
        var components = await BuildComponentsAsync(allowances, ct);

        var feeIncome = await _examFeeIncomeResolver.GetFeeIncomeAsync(staffId, from, to, ct);
        if (feeIncome > 0)
            components.Add(new PayrollPayslipComponent("Examination Fees", feeIncome, IsDeduction: false));

        var unpaidLeaveDays = await _staffLeaveResolver.GetUnpaidLeaveDaysAsync(staffId, from, to, ct);
        var unpaidLeaveDeduction = salary is not null && unpaidLeaveDays > 0
            ? Math.Round(salary.BaseSalary / WorkingDaysPerMonth * unpaidLeaveDays, 2)
            : 0m;

        return new PayrollPayslipDraft(
            staffId,
            baseSalary,
            feeIncome,
            unpaidLeaveDays,
            unpaidLeaveDeduction,
            components);
    }

    private async Task<Salary?> FindBaseSalaryAsync(Guid staffId, DateTime to, CancellationToken ct)
    {
        var spec = new DynamicSpecification<Salary>();
        spec.AddCriteria(s => s.StaffId == staffId);
        spec.AddCriteria(s => s.IsActive);
        spec.AddCriteria(s => s.EffectiveDate.Date <= to.Date);
        spec.ApplyOrderByDescending(s => s.EffectiveDate);

        return (await _salaryRepository.FindAsync(spec, ct)).FirstOrDefault();
    }

    private async Task<IReadOnlyList<AllowanceAssignment>> FindActiveAllowancesAsync(
        Guid staffId,
        DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        var spec = new DynamicSpecification<AllowanceAssignment>();
        spec.AddCriteria(a => a.StaffId == staffId);
        spec.AddCriteria(a => a.IsActive);
        spec.AddCriteria(a => a.EffectiveDate.Date <= to.Date);
        spec.AddCriteria(a => a.EndDate == null || a.EndDate.Value.Date >= from.Date);

        return await _allowanceAssignmentRepository.FindAsync(spec, ct);
    }

    private async Task<List<PayrollPayslipComponent>> BuildComponentsAsync(
        IReadOnlyList<AllowanceAssignment> allowances,
        CancellationToken ct)
    {
        var componentIds = allowances
            .Where(a => a.SalaryComponentId.HasValue)
            .Select(a => a.SalaryComponentId!.Value)
            .Distinct()
            .ToList();

        var lookup = new Dictionary<Guid, SalaryComponent>();
        if (componentIds.Count > 0)
        {
            var spec = new DynamicSpecification<SalaryComponent>();
            spec.AddCriteria(c => componentIds.Contains(c.Id));
            var components = await _salaryComponentRepository.FindAsync(spec, ct);
            lookup = components.ToDictionary(c => c.Id);
        }

        var result = new List<PayrollPayslipComponent>();

        foreach (var allowance in allowances.OrderBy(a => a.EffectiveDate))
        {
            var isDeduction = allowance.SalaryComponentId.HasValue
                && lookup.TryGetValue(allowance.SalaryComponentId!.Value, out var component)
                && component.Kind == ComponentKind.Deduction;

            var amount = allowance.IsPerWorkDay ? allowance.Amount * WorkingDaysPerMonth : allowance.Amount;

            result.Add(new PayrollPayslipComponent(allowance.Name, amount, isDeduction));
        }

        return result;
    }
}