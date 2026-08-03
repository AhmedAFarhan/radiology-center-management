using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetExaminationFees;

public static class GetExaminationFeesQueryHandler
{
    public static async Task<Result<PagedResult<ExaminationFeeDto>>> HandleAsync(
        GetExaminationFeesQuery query,
        IExaminationFeeRepository examinationFeeRepository,
        CancellationToken ct)
    {
        var paged = await examinationFeeRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(f => f.Adapt<ExaminationFeeDto>()).ToList();

        return Result.Success(new PagedResult<ExaminationFeeDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize));
    }
}