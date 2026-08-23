using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Catalog.Application.Queries.ExportExaminationTypes;

public record ExportExaminationTypesQuery(QueryRequest Request, bool? IsActive = null) : IQuery;
