using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.ActivateAllowanceAssignment;

public static class ActivateAllowanceAssignmentCommandHandler
{
    public static async Task<Result> HandleAsync(
        ActivateAllowanceAssignmentCommand command,
        IAllowanceAssignmentRepository allowanceAssignmentRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var assignment = await allowanceAssignmentRepository.GetByIdAsync(command.Id, ct);
        if (assignment is null)
            return Result.Failure(Error.NotFound("AllowanceAssignment", command.Id));

        assignment.Activate();
        allowanceAssignmentRepository.Update(assignment);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}