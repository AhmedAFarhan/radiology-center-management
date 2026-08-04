using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateSalaryComponent;

public static class UpdateSalaryComponentCommandHandler
{
    public static Task<Result> HandleAsync(
        UpdateSalaryComponentCommand command,
        ISalaryComponentRepository salaryComponentRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct) =>
        EntityCommands.UpdateAsync(
            salaryComponentRepository,
            unitOfWork,
            command.SalaryComponentId,
            "SalaryComponent",
            component =>
            {
                var frequency = string.IsNullOrWhiteSpace(command.Frequency)
                    ? null
                    : Frequency.FromName<Frequency>(command.Frequency);

                component.Update(
                    command.Name,
                    ComponentKind.FromName<ComponentKind>(command.Kind),
                    command.IsPercentage,
                    command.DefaultValue,
                    frequency,
                    command.IsPerWorkDay);
            },
            ct);
}