using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateSalaryComponent;

public static class DeactivateSalaryComponentCommandHandler
{
    public static Task<Result> HandleAsync(
        DeactivateSalaryComponentCommand command,
        ISalaryComponentRepository repository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.SetActiveAsync(
            repository,
            unitOfWork,
            command.Id,
            "SalaryComponent",
            component => component.Deactivate(),
            ct);
}