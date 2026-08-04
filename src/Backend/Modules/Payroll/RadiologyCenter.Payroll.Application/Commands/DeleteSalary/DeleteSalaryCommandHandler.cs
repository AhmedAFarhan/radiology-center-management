using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteSalary;

public static class DeleteSalaryCommandHandler
{
    public static Task<Result> HandleAsync(
        DeleteSalaryCommand command,
        ISalaryRepository salaryRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.DeleteAsync(
            salaryRepository,
            unitOfWork,
            command.Id,
            "Salary",
            ct);
}