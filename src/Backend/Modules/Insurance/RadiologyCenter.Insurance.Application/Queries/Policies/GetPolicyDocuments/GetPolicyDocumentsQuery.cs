namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPolicyDocuments;

public record GetPolicyDocumentsQuery(Guid PolicyId) : IQuery;