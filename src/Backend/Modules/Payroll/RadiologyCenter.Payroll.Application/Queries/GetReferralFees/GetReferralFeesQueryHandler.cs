using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Queries.GetReferralFees;

public static class GetReferralFeesQueryHandler
{
    public static async Task<Result<PagedResult<ReferralFeeDto>>> HandleAsync(
        GetReferralFeesQuery query,
        IReferralFeeRepository referralFeeRepository,
        CancellationToken ct)
    {
        var paged = await referralFeeRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(f => f.Adapt<ReferralFeeDto>()).ToList();

        return Result.Success(new PagedResult<ReferralFeeDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize));
    }
}