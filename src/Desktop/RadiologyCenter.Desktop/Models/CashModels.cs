namespace RadiologyCenter.Desktop.Models;

public sealed record CashSessionDto(
    string Id,
    string UserId,
    string UserName,
    string? WorkShiftId,
    string Status,
    decimal OpeningFloat,
    decimal Balance,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    string? Notes,
    int EntryCount,
    string StatusKey = "");

public sealed class OpenCashSessionInput
{
    public decimal OpeningFloat { get; set; }
    public string? WorkShiftId { get; set; }
    public string? Notes { get; set; }
}

public sealed record CashEntryDto(
    string Id,
    string CashSessionId,
    string Direction,
    string Reason,
    decimal Amount,
    string? Description,
    string? ReferenceId,
    DateTime OccurredAt);

public sealed class AddCashEntryInput
{
    public string CashSessionId { get; set; } = string.Empty;
    public string Direction { get; set; } = "In";
    public string Reason { get; set; } = "Payment";
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? ReferenceId { get; set; }
}

public sealed record CashHandoverDto(
    string Id,
    string CashSessionId,
    decimal ExpectedTotal,
    decimal CountedTotal,
    decimal OverShortAmount,
    DateTime ClosedAt,
    string ClosedByUserId,
    string ClosedByName,
    string? ApprovedByUserId,
    DateTime? ApprovedAt,
    string? ReceivingCashSessionId,
    string? Notes);

public sealed class CloseCashSessionInput
{
    public string CashSessionId { get; set; } = string.Empty;
    public decimal CountedTotal { get; set; }
    public string? ReceivingUserId { get; set; }
    public decimal? ReceivingOpeningFloat { get; set; }
    public string? Notes { get; set; }
}