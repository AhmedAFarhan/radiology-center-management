using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateSalaryComponent;

public static class UpdateSalaryComponentCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateSalaryComponentCommand command,
        ISalaryComponentRepository salaryComponentRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var component = await salaryComponentRepository.GetByIdAsync(command.SalaryComponentId, ct);
        if (component is null)
            return Result.Failure(Error.NotFound("SalaryComponent", command.SalaryComponentId));

        var kind = ComponentKind.FromName<ComponentKind>(command.Kind);
        var frequency = string.IsNullOrWhiteSpace(command.Frequency)
            ? null
            : Frequency.FromName<Frequency>(command.Frequency);

        component.Update(command.Name, kind, command.IsPercentage, command.DefaultValue, frequency, command.IsPerWorkDay);

        salaryComponentRepository.Update(component);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}