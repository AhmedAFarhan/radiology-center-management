namespace RadiologyCenter.Examinations.Application.Queries.GetExaminationTypeById;

public record GetExaminationTypeByIdQuery(Guid Id) : IQuery, IEntityIdQuery;
