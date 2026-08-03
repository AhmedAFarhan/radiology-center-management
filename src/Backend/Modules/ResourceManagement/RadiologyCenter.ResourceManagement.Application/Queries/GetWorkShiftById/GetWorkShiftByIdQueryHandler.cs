using Mapster;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetWorkShiftById;

public static class GetWorkShiftByIdQueryHandler
{
    public static async Task<Result<WorkShiftDto>> HandleAsync(
        GetWorkShiftByIdQuery query,
        IWorkShiftRepository workShiftRepository,
        CancellationToken ct)
    {
        var workShift = await workShiftRepository.GetByIdAsync(query.Id, ct);
        if (workShift is null)
            return Result.Failure<WorkShiftDto>(Error.NotFound("WorkShift", query.Id));

        return Result.Success(workShift.Adapt<WorkShiftDto>());
    }
}
