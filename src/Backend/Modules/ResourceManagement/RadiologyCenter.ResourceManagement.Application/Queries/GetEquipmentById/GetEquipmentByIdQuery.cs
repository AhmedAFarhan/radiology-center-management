namespace RadiologyCenter.ResourceManagement.Application.Queries.GetEquipmentById;

public record GetEquipmentByIdQuery(Guid Id) : IQuery, IEntityIdQuery;
