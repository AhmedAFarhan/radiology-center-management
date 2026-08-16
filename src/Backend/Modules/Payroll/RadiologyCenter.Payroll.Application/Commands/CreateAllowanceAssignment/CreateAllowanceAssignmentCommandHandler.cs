using Mapster;
using RadiologyCenter.Payroll.Application.Localization;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.CreateAllowanceAssignment;

public static class CreateAllowanceAssignmentCommandHandler
{
    public static async Task<Result<AllowanceAssignmentDto>> HandleAsync(
        CreateAllowanceAssignmentCommand command,
        IAllowanceAssignmentRepository allowanceAssignmentRepository,
        ISalaryComponentRepository salaryComponentRepository,
        IPayrollStaffDirectory staffDirectory,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (!await staffDirectory.ExistsAsync(command.StaffId, ct))
            return Result.Failure<AllowanceAssignmentDto>(Error.NotFound(ErrorCodes.StaffNotFound, "Staff", command.StaffId));

        if (command.SalaryComponentId.HasValue &&
            await salaryComponentRepository.GetByIdAsync(command.SalaryComponentId.Value, ct) is null)
        {
            return Result.Failure<AllowanceAssignmentDto>(Error.NotFound(ErrorCodes.SalaryComponentNotFound, "SalaryComponent", command.SalaryComponentId.Value));
        }

        var frequency = string.IsNullOrWhiteSpace(command.Frequency)
            ? null
            : Frequency.FromName<Frequency>(command.Frequency);

        var assignment = AllowanceAssignment.Create(
            command.StaffId,
            command.Name,
            command.Amount,
            command.EffectiveDate,
            command.SalaryComponentId,
            frequency,
            command.EndDate,
            command.IsPerWorkDay);

        await allowanceAssignmentRepository.AddAsync(assignment, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(assignment.Adapt<AllowanceAssignmentDto>());
    }
}