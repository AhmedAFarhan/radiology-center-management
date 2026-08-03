using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteSalary;

public static class DeleteSalaryCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteSalaryCommand command,
        ISalaryRepository salaryRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var salary = await salaryRepository.GetByIdAsync(command.Id, ct);
        if (salary is null)
            return Result.Failure(Error.NotFound("Salary", command.Id));

        salaryRepository.Remove(salary);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}