using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Payroll.Application.Queries.GetExaminationFees;

public record GetExaminationFeesQuery(QueryRequest Request) : IQuery;