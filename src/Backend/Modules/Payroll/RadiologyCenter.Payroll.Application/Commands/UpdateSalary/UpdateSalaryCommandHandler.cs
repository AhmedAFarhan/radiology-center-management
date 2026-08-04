using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateSalary;

public static class UpdateSalaryCommandHandler
{
    public static Task<Result> HandleAsync(
        UpdateSalaryCommand command,
        ISalaryRepository salaryRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.UpdateAsync(
            salaryRepository,
            unitOfWork,
            command.SalaryId,
            "Salary",
            salary => salary.Update(
                command.BaseSalary,
                SalaryType.FromName<SalaryType>(command.SalaryType),
                command.EffectiveDate),
            ct);
}