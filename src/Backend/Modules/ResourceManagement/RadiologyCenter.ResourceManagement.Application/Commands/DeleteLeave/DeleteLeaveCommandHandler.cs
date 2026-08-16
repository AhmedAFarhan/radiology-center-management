using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.ResourceManagement.Application.Commands.DeleteLeave;

public static class DeleteLeaveCommandHandler
{
    public static async Task<Result> HandleAsync(
        DeleteLeaveCommand command,
        ILeaveRepository leaveRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var leave = await leaveRepository.GetByIdAsync(command.LeaveId, ct);
        if (leave is null)
            return Result.Failure(Error.NotFound(ErrorCodes.LeaveNotFound, "Leave", command.LeaveId));

        leaveRepository.Remove(leave);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
