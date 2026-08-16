using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Cash.Domain.Errors;

namespace RadiologyCenter.Cash.Domain.Entities;

public sealed class CashHandover : SoftDeletableAggregateRoot<Guid>
{
    public Guid CashSessionId { get; private set; }
    public decimal ExpectedTotal { get; private set; }
    public decimal CountedTotal { get; private set; }
    public decimal OverShortAmount { get; private set; }
    public DateTime ClosedAt { get; private set; }
    public Guid ClosedByUserId { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public Guid? ReceivingCashSessionId { get; private set; }
    public string? Notes { get; private set; }

    private CashHandover() { }

    public static CashHandover Create(
        Guid cashSessionId,
        decimal expectedTotal,
        decimal countedTotal,
        DateTime closedAt,
        Guid closedByUserId,
        string? notes = null)
    {
        Guard.AgainstEmpty(cashSessionId, nameof(cashSessionId));
        Guard.Against(countedTotal, c => c < 0, DomainErrors.CountedTotalNegative, "Counted total cannot be negative.");
        Guard.Against(expectedTotal, e => e < 0, DomainErrors.ExpectedTotalNegative, "Expected total cannot be negative.");
        Guard.AgainstDefault(closedAt, nameof(closedAt));
        Guard.AgainstEmpty(closedByUserId, nameof(closedByUserId));

        return new CashHandover
        {
            Id = Guid.NewGuid(),
            CashSessionId = cashSessionId,
            ExpectedTotal = expectedTotal,
            CountedTotal = countedTotal,
            OverShortAmount = countedTotal - expectedTotal,
            ClosedAt = closedAt,
            ClosedByUserId = closedByUserId,
            Notes = notes?.Trim()
        };
    }

    public void Approve(Guid approvedByUserId, DateTime approvedAt)
    {
        Guard.AgainstEmpty(approvedByUserId, nameof(approvedByUserId));
        ApprovedByUserId = approvedByUserId;
        ApprovedAt = approvedAt;
    }

    public void SetReceivingSession(Guid receivingCashSessionId)
    {
        Guard.AgainstEmpty(receivingCashSessionId, nameof(receivingCashSessionId));
        ReceivingCashSessionId = receivingCashSessionId;
    }
}