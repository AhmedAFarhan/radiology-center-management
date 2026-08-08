namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetInsurancePolicyById;

public record GetInsurancePolicyByIdQuery(Guid PolicyId) : IQuery;