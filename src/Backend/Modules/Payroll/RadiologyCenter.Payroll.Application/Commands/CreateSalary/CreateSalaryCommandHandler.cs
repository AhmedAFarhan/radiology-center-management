using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.CreateSalary;

public static class CreateSalaryCommandHandler
{
    public static async Task<Result<SalaryDto>> HandleAsync(
        CreateSalaryCommand command,
        ISalaryRepository salaryRepository,
        IPayrollStaffDirectory staffDirectory,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (!await staffDirectory.ExistsAsync(command.StaffId, ct))
            return Result.Failure<SalaryDto>(Error.NotFound("Staff", command.StaffId));

        var salaryType = SalaryType.FromName<SalaryType>(command.SalaryType);

        var salary = Salary.Create(command.StaffId, command.BaseSalary, salaryType, command.EffectiveDate);

        if (salary.IsActive)
        {
            var previousSalaries = await FindActiveSalariesAsync(salaryRepository, command.StaffId, ct);
            foreach (var previous in previousSalaries)
            {
                previous.Deactivate();
                salaryRepository.Update(previous);
            }
        }

        await salaryRepository.AddAsync(salary, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(salary.Adapt<SalaryDto>());
    }

    private static async Task<IReadOnlyList<Salary>> FindActiveSalariesAsync(
        ISalaryRepository repository,
        Guid staffId,
        CancellationToken ct)
    {
        var spec = new DynamicSpecification<Salary>();
        spec.AddCriteria(s => s.StaffId == staffId);
        spec.AddCriteria(s => s.IsActive);
        return await repository.FindAsync(spec, ct);
    }
}