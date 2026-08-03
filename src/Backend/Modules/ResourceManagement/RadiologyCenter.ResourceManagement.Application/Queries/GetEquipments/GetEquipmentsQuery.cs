using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetEquipments;

public record GetEquipmentsQuery(QueryRequest Request) : IQuery;
