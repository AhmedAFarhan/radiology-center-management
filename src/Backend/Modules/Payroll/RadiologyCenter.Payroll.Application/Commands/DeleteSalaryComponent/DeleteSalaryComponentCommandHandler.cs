using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteSalaryComponent;

public static class DeleteSalaryComponentCommandHandler
{
    public static Task<Result> HandleAsync(
        DeleteSalaryComponentCommand command,
        ISalaryComponentRepository repository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.DeleteAsync(
            repository,
            unitOfWork,
            command.Id,
            "SalaryComponent",
            ct);
}