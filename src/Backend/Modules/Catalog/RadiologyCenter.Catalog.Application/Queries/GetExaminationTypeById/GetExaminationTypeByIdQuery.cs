namespace RadiologyCenter.Catalog.Application.Queries.GetExaminationTypeById;

public record GetExaminationTypeByIdQuery(Guid Id) : IQuery, IEntityIdQuery;
