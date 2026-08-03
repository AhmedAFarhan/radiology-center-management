using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryById;

public record GetSalaryByIdQuery(Guid Id) : IQuery, IEntityIdQuery;