using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryComponentById;

public record GetSalaryComponentByIdQuery(Guid Id) : IQuery, IEntityIdQuery;