using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Queries.GetExaminationFeeById;

public record GetExaminationFeeByIdQuery(Guid Id) : IQuery, IEntityIdQuery;