using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Commands.DeleteAllowanceAssignment;

public static class DeleteAllowanceAssignmentCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteAllowanceAssignmentCommand command,
        IAllowanceAssignmentRepository allowanceAssignmentRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var assignment = await allowanceAssignmentRepository.GetByIdAsync(command.Id, ct);
        if (assignment is null)
            return Result.Failure(Error.NotFound("AllowanceAssignment", command.Id));

        allowanceAssignmentRepository.Remove(assignment);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}