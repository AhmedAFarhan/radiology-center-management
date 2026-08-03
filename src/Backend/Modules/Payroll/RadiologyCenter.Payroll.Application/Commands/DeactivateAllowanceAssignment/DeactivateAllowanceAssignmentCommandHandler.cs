using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.DeactivateAllowanceAssignment;

public static class DeactivateAllowanceAssignmentCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeactivateAllowanceAssignmentCommand command,
        IAllowanceAssignmentRepository allowanceAssignmentRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var assignment = await allowanceAssignmentRepository.GetByIdAsync(command.Id, ct);
        if (assignment is null)
            return Result.Failure(Error.NotFound("AllowanceAssignment", command.Id));

        assignment.Deactivate();
        allowanceAssignmentRepository.Update(assignment);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}