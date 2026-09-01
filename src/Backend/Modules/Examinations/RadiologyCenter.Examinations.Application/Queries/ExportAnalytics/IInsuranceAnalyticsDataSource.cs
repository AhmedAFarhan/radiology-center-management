using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public interface IInsuranceAnalyticsDataSource
{
    Task<IReadOnlyList<InsuranceClaimRowDto>> GetClaimRowsAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<int> GetTotalClaimsAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<int> GetClaimsByStatusAsync(string status, DateTime from, DateTime to, CancellationToken ct = default);
    Task<decimal> GetTotalBilledAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<decimal> GetTotalPayerShareAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<decimal> GetTotalPatientShareAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<decimal> GetTotalSettledAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<decimal> GetOutstandingAmountAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
