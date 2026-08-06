using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Examinations.Application.Queries.GetStaffMachineAnalytics;

public record GetStaffMachineAnalyticsQuery(DateTime? From, DateTime? To) : IQuery;