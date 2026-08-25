using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Common;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Services;

public class PayrollPayslipCalculator : IPayslipCalculator
{
    private readonly ISalaryRepository _salaryRepository;
    private readonly IAllowanceAssignmentRepository _allowanceAssignmentRepository;
    private readonly ISalaryComponentRepository _salaryComponentRepository;
    private readonly IExamFeeIncomeResolver _examFeeIncomeResolver;
    private readonly IStaffLeaveResolver _staffLeaveResolver;
    private readonly IStaffWorkHoursResolver _staffWorkHoursResolver;
    private readonly IStaffRepository _staffRepository;

    public PayrollPayslipCalculator(
        ISalaryRepository salaryRepository,
        IAllowanceAssignmentRepository allowanceAssignmentRepository,
        ISalaryComponentRepository salaryComponentRepository,
        IExamFeeIncomeResolver examFeeIncomeResolver,
        IStaffLeaveResolver staffLeaveResolver,
        IStaffWorkHoursResolver staffWorkHoursResolver,
        IStaffRepository staffRepository)
    {
        _salaryRepository = salaryRepository;
        _allowanceAssignmentRepository = allowanceAssignmentRepository;
        _salaryComponentRepository = salaryComponentRepository;
        _examFeeIncomeResolver = examFeeIncomeResolver;
        _staffLeaveResolver = staffLeaveResolver;
        _staffWorkHoursResolver = staffWorkHoursResolver;
        _staffRepository = staffRepository;
    }

    public async Task<PayrollPayslipDraft?> CalculateAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct)
    {
        var salary = await FindBaseSalaryAsync(staffId, to, ct);

        var runWorkingDays = PayrollCalendar.WorkingDaysBetween(from.Date, to.Date);
        var baseSalary = await ComputeBaseSalaryAsync(salary, staffId, from, to, ct);

        var unpaidLeaveDays = await _staffLeaveResolver.GetUnpaidLeaveDaysAsync(staffId, from, to, ct);
        var unpaidLeaveDeduction = salary is not null
            && salary.SalaryType != SalaryType.Hourly
            && runWorkingDays > 0
            && unpaidLeaveDays > 0
            ? Math.Round(salary.BaseSalary / runWorkingDays * unpaidLeaveDays, 2, MidpointRounding.AwayFromZero)
            : 0m;

        var allowances = await FindActiveAllowancesAsync(staffId, from, to, ct);
        var components = await BuildComponentsAsync(allowances, from, to, runWorkingDays, ct);

        var feeIncome = await _examFeeIncomeResolver.GetFeeIncomeAsync(staffId, from, to, ct);

        var staff = await _staffRepository.GetByIdAsync(staffId, ct);
        var calculationRule = staff?.SalaryCalculationRule ?? SalaryCalculationRule.FixedPlusFees;

        if (calculationRule == SalaryCalculationRule.HigherOfFixedOrFees)
        {
            baseSalary = Math.Max(baseSalary, feeIncome);
            feeIncome = 0m;
        }

        if (feeIncome > 0)
            components.Add(new PayrollPayslipComponent("Examination Fees", feeIncome, IsDeduction: false));

        return new PayrollPayslipDraft(
            staffId,
            baseSalary,
            feeIncome,
            unpaidLeaveDays,
            unpaidLeaveDeduction,
            components);
    }

    private async Task<decimal> ComputeBaseSalaryAsync(
        Salary? salary,
        Guid staffId,
        DateTime from,
        DateTime to,
        CancellationToken ct)
    {
        if (salary is null)
            return 0m;

        if (salary.SalaryType == SalaryType.Hourly)
        {
            var workedHours = await _staffWorkHoursResolver.GetWorkedHoursAsync(staffId, from, to, ct);
            return Math.Round(salary.BaseSalary * workedHours, 2, MidpointRounding.AwayFromZero);
        }

        return salary.BaseSalary;
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
        DateTime from,
        DateTime to,
        int runWorkingDays,
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
            var component = allowance.SalaryComponentId.HasValue
                ? lookup.GetValueOrDefault(allowance.SalaryComponentId.Value)
                : null;

            if (!IsDueInRun(allowance, component, from, to))
                continue;

            var isDeduction = allowance.SalaryComponentId.HasValue
                && lookup.TryGetValue(allowance.SalaryComponentId!.Value, out var lookupComponent)
                && lookupComponent.Kind == ComponentKind.Deduction;

            var amount = allowance.IsPerWorkDay ? allowance.Amount * runWorkingDays : allowance.Amount;

            result.Add(new PayrollPayslipComponent(allowance.Name, amount, isDeduction));
        }

        return result;
    }

    private static bool IsDueInRun(AllowanceAssignment allowance, SalaryComponent? component, DateTime from, DateTime to)
    {
        var frequency = allowance.Frequency ?? component?.Frequency ?? Frequency.Monthly;
        if (frequency == Frequency.Monthly)
            return true;

        var effective = allowance.EffectiveDate.Date;
        var fromDate = from.Date;
        var toDate = to.Date;

        if (frequency == Frequency.OneTime)
            return effective >= fromDate && effective <= toDate;

        if (frequency == Frequency.Quarterly)
        {
            var quarterStart = new DateTime(toDate.Year, ((toDate.Month - 1) / 3) * 3 + 1, 1);
            return quarterStart >= fromDate && quarterStart <= toDate && quarterStart >= effective;
        }

        var yearStart = new DateTime(toDate.Year, 1, 1);
        return yearStart >= fromDate && yearStart <= toDate && yearStart >= effective;
    }
}