using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Queries.GetAllowanceAssignmentById;

public record GetAllowanceAssignmentByIdQuery(Guid Id) : IQuery, IEntityIdQuery;