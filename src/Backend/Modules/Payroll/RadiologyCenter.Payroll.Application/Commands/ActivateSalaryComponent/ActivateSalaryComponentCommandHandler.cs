using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateSalaryComponent;

public static class ActivateSalaryComponentCommandHandler
{
    public static async Task<Result> HandleAsync(
        ActivateSalaryComponentCommand command,
        ISalaryComponentRepository salaryComponentRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var component = await salaryComponentRepository.GetByIdAsync(command.Id, ct);
        if (component is null)
            return Result.Failure(Error.NotFound("SalaryComponent", command.Id));

        component.Activate();
        salaryComponentRepository.Update(component);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}