using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetReferralDoctors;

public record GetReferralDoctorsQuery(QueryRequest Request) : IQuery;
