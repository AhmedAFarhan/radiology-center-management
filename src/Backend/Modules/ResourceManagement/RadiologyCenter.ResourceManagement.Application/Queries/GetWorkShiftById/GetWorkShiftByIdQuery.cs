namespace RadiologyCenter.ResourceManagement.Application.Queries.GetWorkShiftById;

public record GetWorkShiftByIdQuery(Guid Id) : IQuery, IEntityIdQuery;
