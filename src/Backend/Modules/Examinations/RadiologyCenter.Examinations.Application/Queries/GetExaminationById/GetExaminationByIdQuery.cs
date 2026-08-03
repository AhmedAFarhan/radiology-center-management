namespace RadiologyCenter.Examinations.Application.Queries.GetExaminationById;

public record GetExaminationByIdQuery(Guid Id) : IQuery, IEntityIdQuery;
