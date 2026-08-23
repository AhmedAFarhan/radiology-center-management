using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Patients.Application.Queries.ExportPatients;

public record ExportPatientsQuery(QueryRequest Request, bool? IsActive = null) : IQuery;
