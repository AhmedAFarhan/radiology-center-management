namespace RadiologyCenter.Insurance.Application.Queries.Claims.GetClaimByExamination;

public record GetClaimByExaminationQuery(Guid ExaminationId) : IQuery;