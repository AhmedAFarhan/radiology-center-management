using Mapster;
using RadiologyCenter.ResourceManagement.Application.Localization;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetLeaveById;

public static class GetLeaveByIdQueryHandler
{
    public static async Task<Result<LeaveDto>> HandleAsync(
        GetLeaveByIdQuery query,
        ILeaveRepository leaveRepository,
        CancellationToken ct)
    {
        var leave = await leaveRepository.GetByIdAsync(query.Id, ct);
        if (leave is null)
            return Result.Failure<LeaveDto>(Error.NotFound(ErrorCodes.LeaveNotFound, "Leave", query.Id));

        return Result.Success(leave.Adapt<LeaveDto>());
    }
}
