using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Examinations.Application.Queries.GetOperationalAnalytics;

public record GetOperationalAnalyticsQuery(DateTime? From, DateTime? To) : IQuery;