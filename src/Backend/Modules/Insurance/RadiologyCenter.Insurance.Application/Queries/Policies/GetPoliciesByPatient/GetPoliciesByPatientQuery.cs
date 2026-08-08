namespace RadiologyCenter.Insurance.Application.Queries.Policies.GetPoliciesByPatient;

public record GetPoliciesByPatientQuery(Guid PatientId) : IQuery;