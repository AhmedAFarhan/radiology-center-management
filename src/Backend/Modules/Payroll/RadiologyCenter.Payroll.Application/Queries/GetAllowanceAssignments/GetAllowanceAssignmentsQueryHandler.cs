using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetAllowanceAssignments;

public static class GetAllowanceAssignmentsQueryHandler
{
    public static async Task<Result<PagedResult<AllowanceAssignmentDto>>> HandleAsync(
        GetAllowanceAssignmentsQuery query,
        IAllowanceAssignmentRepository allowanceAssignmentRepository,
        CancellationToken ct)
    {
        var paged = await allowanceAssignmentRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(a => a.Adapt<AllowanceAssignmentDto>()).ToList();

        return Result.Success(new PagedResult<AllowanceAssignmentDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize));
    }
}