using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateSalary;

public static class ActivateSalaryCommandHandler
{
    public static Task<Result> HandleAsync(
        ActivateSalaryCommand command,
        ISalaryRepository salaryRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.SetActiveAsync(
            salaryRepository,
            unitOfWork,
            command.Id,
            "Salary",
            salary => salary.Activate(),
            ct);
}