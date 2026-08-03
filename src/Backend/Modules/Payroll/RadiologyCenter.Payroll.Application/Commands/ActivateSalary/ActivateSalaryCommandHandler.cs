using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateSalary;

public static class ActivateSalaryCommandHandler
{
    public static async Task<Result> HandleAsync(
        ActivateSalaryCommand command,
        ISalaryRepository salaryRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var salary = await salaryRepository.GetByIdAsync(command.Id, ct);
        if (salary is null)
            return Result.Failure(Error.NotFound("Salary", command.Id));

        salary.Activate();
        salaryRepository.Update(salary);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}