using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Payroll.Application.Queries.GetSalaryComponents;

public record GetSalaryComponentsQuery(QueryRequest Request) : IQuery;