using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Patients.Application.Queries.GetPatients;

public record GetPatientsQuery(QueryRequest Request) : IQuery;
