using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaries;

public record GetSalariesQuery(QueryRequest Request) : IQuery;