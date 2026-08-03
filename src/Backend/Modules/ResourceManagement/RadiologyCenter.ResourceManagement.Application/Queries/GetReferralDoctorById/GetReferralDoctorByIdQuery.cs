namespace RadiologyCenter.ResourceManagement.Application.Queries.GetReferralDoctorById;

public record GetReferralDoctorByIdQuery(Guid Id) : IQuery, IEntityIdQuery;
