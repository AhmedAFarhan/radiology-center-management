using Mapster;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.CreateSalaryComponent;

public static class CreateSalaryComponentCommandHandler
{
    public static async Task<Result<SalaryComponentDto>> HandleAsync(
        CreateSalaryComponentCommand command,
        ISalaryComponentRepository salaryComponentRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var kind = ComponentKind.FromName<ComponentKind>(command.Kind);
        var frequency = string.IsNullOrWhiteSpace(command.Frequency)
            ? null
            : Frequency.FromName<Frequency>(command.Frequency);

        var component = SalaryComponent.Create(
            command.Name,
            kind,
            command.IsPercentage,
            command.DefaultValue,
            frequency,
            command.IsPerWorkDay);

        await salaryComponentRepository.AddAsync(component, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(component.Adapt<SalaryComponentDto>());
    }
}