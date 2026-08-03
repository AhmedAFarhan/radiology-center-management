using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateSalary;

public static class UpdateSalaryCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateSalaryCommand command,
        ISalaryRepository salaryRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var salary = await salaryRepository.GetByIdAsync(command.SalaryId, ct);
        if (salary is null)
            return Result.Failure(Error.NotFound("Salary", command.SalaryId));

        var salaryType = SalaryType.FromName<SalaryType>(command.SalaryType);
        salary.Update(command.BaseSalary, salaryType, command.EffectiveDate);

        salaryRepository.Update(salary);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}