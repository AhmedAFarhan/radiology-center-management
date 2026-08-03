using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteSalaryComponent;

public static class DeleteSalaryComponentCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteSalaryComponentCommand command,
        ISalaryComponentRepository salaryComponentRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var component = await salaryComponentRepository.GetByIdAsync(command.Id, ct);
        if (component is null)
            return Result.Failure(Error.NotFound("SalaryComponent", command.Id));

        salaryComponentRepository.Remove(component);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}