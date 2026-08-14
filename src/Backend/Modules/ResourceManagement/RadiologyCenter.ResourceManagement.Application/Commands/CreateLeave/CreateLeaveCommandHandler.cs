using Mapster;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateLeave;

public static class CreateLeaveCommandHandler
{
    public static async Task<Result<LeaveDto>> HandleAsync(
        CreateLeaveCommand command,
        ILeaveRepository leaveRepository,
        IStaffRepository staffRepository,
        IResourceManagementUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var staff = await staffRepository.GetByIdAsync(command.StaffId, ct);
        if (staff is null)
            return Result.Failure<LeaveDto>(Error.NotFound("Staff", command.StaffId));

        if (await leaveRepository.HasOverlapAsync(command.StaffId, command.StartDate, command.EndDate, ct: ct))
            return Result.Failure<LeaveDto>(Error.Conflict("The staff member already has leave overlapping the requested period."));

        var leaveType = LeaveType.FromName<LeaveType>(command.LeaveType);

        var leave = Leave.Create(
            command.StaffId,
            leaveType,
            command.StartDate,
            command.EndDate,
            command.Reason);

        await leaveRepository.AddAsync(leave, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(leave.Adapt<LeaveDto>());
    }
}
