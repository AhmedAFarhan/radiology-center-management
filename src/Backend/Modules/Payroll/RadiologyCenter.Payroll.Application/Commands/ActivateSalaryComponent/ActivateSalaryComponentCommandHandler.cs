using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateSalaryComponent;

public static class ActivateSalaryComponentCommandHandler
{
    public static Task<Result> HandleAsync(
        ActivateSalaryComponentCommand command,
        ISalaryComponentRepository repository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.SetActiveAsync(
            repository,
            unitOfWork,
            command.Id,
            "SalaryComponent",
            component => component.Activate(),
            ct);
}