using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Queries.GetPayRunById;

public record GetPayRunByIdQuery(Guid Id) : IQuery, IEntityIdQuery;