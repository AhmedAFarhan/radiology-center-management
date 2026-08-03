using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateSalaryComponent;

public static class DeactivateSalaryComponentCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeactivateSalaryComponentCommand command,
        ISalaryComponentRepository salaryComponentRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var component = await salaryComponentRepository.GetByIdAsync(command.Id, ct);
        if (component is null)
            return Result.Failure(Error.NotFound("SalaryComponent", command.Id));

        component.Deactivate();
        salaryComponentRepository.Update(component);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}