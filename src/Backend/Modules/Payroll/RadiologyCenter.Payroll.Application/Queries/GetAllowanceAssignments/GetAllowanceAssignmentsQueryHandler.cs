using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Commands.Common;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;

namespace RadiologyCenter.Payroll.Application.Queries.GetAllowanceAssignments;

public static class GetAllowanceAssignmentsQueryHandler
{
    public static Task<Result<PagedResult<AllowanceAssignmentDto>>> HandleAsync(
        GetAllowanceAssignmentsQuery query,
        IAllowanceAssignmentRepository repository,
        CancellationToken ct) =>
        EntityCommands.GetPagedAsync<AllowanceAssignment, AllowanceAssignmentDto>(repository, query.Request, ct);
}