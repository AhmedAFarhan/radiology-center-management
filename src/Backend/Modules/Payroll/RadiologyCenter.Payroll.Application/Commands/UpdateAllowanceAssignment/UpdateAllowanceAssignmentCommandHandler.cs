using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.UpdateAllowanceAssignment;

public static class UpdateAllowanceAssignmentCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateAllowanceAssignmentCommand command,
        IAllowanceAssignmentRepository allowanceAssignmentRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var assignment = await allowanceAssignmentRepository.GetByIdAsync(command.AllowanceAssignmentId, ct);
        if (assignment is null)
            return Result.Failure(Error.NotFound("AllowanceAssignment", command.AllowanceAssignmentId));

        var frequency = string.IsNullOrWhiteSpace(command.Frequency)
            ? null
            : Frequency.FromName<Frequency>(command.Frequency);

        assignment.Update(
            command.Name,
            command.Amount,
            command.EffectiveDate,
            frequency,
            command.EndDate,
            command.IsPerWorkDay);

        allowanceAssignmentRepository.Update(assignment);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}