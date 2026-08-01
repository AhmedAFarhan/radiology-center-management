using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminationTypes;

public record GetExaminationTypesQuery(QueryRequest Request) : IQuery;
