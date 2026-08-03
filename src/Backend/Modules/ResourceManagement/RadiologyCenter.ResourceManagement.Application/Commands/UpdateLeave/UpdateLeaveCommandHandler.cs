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
            return Result.Failure(Error.NotFound("Leave", command.LeaveId));

        var staff = await staffRepository.GetByIdAsync(command.StaffId, ct);
        if (staff is null)
            return Result.Failure(Error.NotFound("Staff", command.StaffId));

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
