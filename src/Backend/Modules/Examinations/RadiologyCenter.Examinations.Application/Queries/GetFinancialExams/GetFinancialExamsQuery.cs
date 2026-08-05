using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.Examinations.Application.Queries.GetFinancialExams;

public record GetFinancialExamsQuery(DateTime? From, DateTime? To) : IQuery;