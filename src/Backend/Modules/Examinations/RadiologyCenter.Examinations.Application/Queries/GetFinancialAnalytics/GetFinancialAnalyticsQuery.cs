using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Examinations.Application.Queries.GetFinancialAnalytics;

public record GetFinancialAnalyticsQuery(DateTime? From, DateTime? To) : IQuery;