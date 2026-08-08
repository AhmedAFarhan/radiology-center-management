namespace RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimById;

public record GetClaimByIdQuery(Guid ClaimId) : IQuery;