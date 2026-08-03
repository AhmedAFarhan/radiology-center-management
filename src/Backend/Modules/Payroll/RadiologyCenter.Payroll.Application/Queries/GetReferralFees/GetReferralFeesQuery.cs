using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Payroll.Application.Queries.GetReferralFees;

public record GetReferralFeesQuery(QueryRequest Request) : IQuery;