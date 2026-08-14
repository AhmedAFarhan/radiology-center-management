using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Catalog.Application.Queries.GetExaminationTypes;

public record GetExaminationTypesQuery(QueryRequest Request, bool? IsActive = null) : IQuery;
