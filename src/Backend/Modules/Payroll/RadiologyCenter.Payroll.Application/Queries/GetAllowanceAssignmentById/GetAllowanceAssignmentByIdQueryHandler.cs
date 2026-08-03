using Mapster;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetAllowanceAssignmentById;

public static class GetAllowanceAssignmentByIdQueryHandler
{
    public static async Task<Result<AllowanceAssignmentDto>> HandleAsync(
        GetAllowanceAssignmentByIdQuery query,
        IAllowanceAssignmentRepository allowanceAssignmentRepository,
        CancellationToken ct)
    {
        var assignment = await allowanceAssignmentRepository.GetByIdAsync(query.Id, ct);
        if (assignment is null)
            return Result.Failure<AllowanceAssignmentDto>(Error.NotFound("AllowanceAssignment", query.Id));

        return Result.Success(assignment.Adapt<AllowanceAssignmentDto>());
    }
}