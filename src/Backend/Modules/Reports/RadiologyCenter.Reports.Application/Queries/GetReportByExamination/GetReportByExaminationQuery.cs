using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Reports.Application.Queries.GetReportByExamination;

public record GetReportByExaminationQuery(Guid ExaminationId) : IQuery;