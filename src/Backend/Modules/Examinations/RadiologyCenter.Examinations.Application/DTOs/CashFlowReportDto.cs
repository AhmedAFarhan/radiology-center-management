namespace RadiologyCenter.Examinations.Application.DTOs;

public sealed record CashFlowReportDto(
    decimal TotalInflows,
    decimal TotalOutflows,
    decimal NetCashFlow,
    int TotalSessions,
    int TotalEntries,
    decimal AvgSessionBalance,
    IReadOnlyList<CashFlowPeriodDto> ByMonth,
    IReadOnlyList<CashFlowEntryTypeDto> ByReason,
    IReadOnlyList<CashFlowSessionSummaryDto> SessionSummaries);

public sealed record CashFlowPeriodDto(
    string Month,
    decimal Inflows,
    decimal Outflows,
    decimal Net);

public sealed record CashFlowEntryTypeDto(
    string Reason,
    decimal InflowAmount,
    decimal OutflowAmount,
    int EntryCount);

public sealed record CashFlowSessionSummaryDto(
    Guid SessionId,
    string UserName,
    decimal OpeningFloat,
    decimal Balance,
    int EntryCount,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    string Status);
