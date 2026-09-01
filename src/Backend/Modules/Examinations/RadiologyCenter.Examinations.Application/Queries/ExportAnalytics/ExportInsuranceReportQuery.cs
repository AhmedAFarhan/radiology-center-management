using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.Examinations.Application.Reports;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public record ExportInsuranceReportQuery(DateTime? From, DateTime? To, ReportFormat Format) : IQuery;
