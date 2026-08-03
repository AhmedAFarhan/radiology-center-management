namespace RadiologyCenter.Patients.Application.Queries.GetPatientById;

public record GetPatientByIdQuery(Guid Id) : IQuery, IEntityIdQuery;
