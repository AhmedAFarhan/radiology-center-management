using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetInsurancePolicies;

public record GetInsurancePoliciesQuery(QueryRequest Request) : IQuery;