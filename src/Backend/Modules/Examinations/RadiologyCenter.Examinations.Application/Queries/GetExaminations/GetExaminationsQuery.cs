using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminations;

public record GetExaminationsQuery(QueryRequest Request) : IQuery;
