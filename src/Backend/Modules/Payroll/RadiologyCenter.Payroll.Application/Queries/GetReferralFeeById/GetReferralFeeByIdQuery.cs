using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Queries.GetReferralFeeById;

public record GetReferralFeeByIdQuery(Guid Id) : IQuery, IEntityIdQuery;