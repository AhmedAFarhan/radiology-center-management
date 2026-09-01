using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public record GetCashFlowReportQuery(DateTime? From, DateTime? To) : IQuery;
