using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateSalary;

public static class DeactivateSalaryCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeactivateSalaryCommand command,
        ISalaryRepository salaryRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var salary = await salaryRepository.GetByIdAsync(command.Id, ct);
        if (salary is null)
            return Result.Failure(Error.NotFound("Salary", command.Id));

        salary.Deactivate();
        salaryRepository.Update(salary);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}