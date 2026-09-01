using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.Examinations.Application.Reports;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public record ExportFinancialReportQuery(DateTime? From, DateTime? To, ReportFormat Format) : IQuery;
