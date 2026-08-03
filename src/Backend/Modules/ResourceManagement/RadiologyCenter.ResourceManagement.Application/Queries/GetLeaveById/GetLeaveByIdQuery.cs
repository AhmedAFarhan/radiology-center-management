namespace RadiologyCenter.ResourceManagement.Application.Queries.GetLeaveById;

public record GetLeaveByIdQuery(Guid Id) : IQuery, IEntityIdQuery;
