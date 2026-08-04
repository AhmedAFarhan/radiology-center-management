using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateSalary;

public static class DeactivateSalaryCommandHandler
{
    public static Task<Result> HandleAsync(
        DeactivateSalaryCommand command,
        ISalaryRepository salaryRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.SetActiveAsync(
            salaryRepository,
            unitOfWork,
            command.Id,
            "Salary",
            salary => salary.Deactivate(),
            ct);
}