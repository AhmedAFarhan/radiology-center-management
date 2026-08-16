using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateLeave;

public static class UpdateLeaveCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateLeaveCommand command,
        ILeaveRepository leaveRepository,
        IStaffRepository staffRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var leave = await leaveRepository.GetByIdAsync(command.LeaveId, ct);
        if (leave is null)
            return Result.Failure(Error.NotFound(ErrorCodes.LeaveNotFound, "Leave", command.LeaveId));

        var staff = await staffRepository.GetByIdAsync(command.StaffId, ct);
        if (staff is null)
            return Result.Failure(Error.NotFound(ErrorCodes.StaffNotFound, "Staff", command.StaffId));

        if (await leaveRepository.HasOverlapAsync(command.StaffId, command.StartDate, command.EndDate, command.LeaveId, ct))
            return Result.Failure(Error.Conflict(ErrorCodes.LeaveOverlap, "The staff member already has leave overlapping the requested period."));

        var leaveType = LeaveType.FromName<LeaveType>(command.LeaveType);

        leave.Update(
            command.StaffId,
            leaveType,
            command.StartDate,
            command.EndDate,
            command.Reason);

        leaveRepository.Update(leave);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
