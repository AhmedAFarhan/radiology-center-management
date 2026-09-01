using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public interface ICashFlowDataSource
{
    Task<IReadOnlyList<CashFlowPeriodDto>> GetByMonthAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<IReadOnlyList<CashFlowEntryTypeDto>> GetByReasonAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<IReadOnlyList<CashFlowSessionSummaryDto>> GetSessionSummariesAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<decimal> GetTotalInflowsAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<decimal> GetTotalOutflowsAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<int> GetTotalSessionsAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<int> GetTotalEntriesAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
