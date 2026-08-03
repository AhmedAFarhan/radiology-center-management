using Mapster;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Application.DTOs;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetStaffById;

public static class GetStaffByIdQueryHandler
{
    public static async Task<Result<StaffDto>> HandleAsync(
        GetStaffByIdQuery query,
        IStaffRepository staffRepository,
        CancellationToken ct)
    {
        var staff = await staffRepository.GetByIdAsync(query.Id, ct);
        if (staff is null)
            return Result.Failure<StaffDto>(Error.NotFound("Staff", query.Id));

        return Result.Success(staff.Adapt<StaffDto>());
    }
}
