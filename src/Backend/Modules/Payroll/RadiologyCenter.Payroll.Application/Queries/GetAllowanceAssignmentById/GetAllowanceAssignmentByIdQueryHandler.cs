using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetAllowanceAssignmentById;

public static class GetAllowanceAssignmentByIdQueryHandler
{
    public static Task<Result<AllowanceAssignmentDto>> HandleAsync(
        GetAllowanceAssignmentByIdQuery query,
        IAllowanceAssignmentRepository repository,
        CancellationToken ct) =>
        EntityCommands.GetByIdAsync<AllowanceAssignment, AllowanceAssignmentDto>(
            repository,
            query.Id,
            "AllowanceAssignment",
            ct);
}