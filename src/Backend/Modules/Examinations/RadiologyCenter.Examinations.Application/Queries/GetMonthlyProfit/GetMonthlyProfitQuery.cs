using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Examinations.Application.Queries.GetMonthlyProfit;

public record GetMonthlyProfitQuery(DateTime? From, DateTime? To) : IQuery;