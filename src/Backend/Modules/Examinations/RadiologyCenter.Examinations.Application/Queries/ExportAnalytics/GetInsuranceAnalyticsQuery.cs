using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public record GetInsuranceAnalyticsQuery(DateTime? From, DateTime? To) : IQuery;
