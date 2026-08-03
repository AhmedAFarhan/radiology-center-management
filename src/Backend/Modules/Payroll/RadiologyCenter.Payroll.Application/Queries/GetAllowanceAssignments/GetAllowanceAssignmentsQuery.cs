using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Payroll.Application.Queries.GetAllowanceAssignments;

public record GetAllowanceAssignmentsQuery(QueryRequest Request) : IQuery;