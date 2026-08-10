using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Cash.Domain.Enumerations;

namespace RadiologyCenter.Cash.Domain.Entities;

public sealed class CashEntry : SoftDeletableAggregateRoot<Guid>
{
    public Guid CashSessionId { get; private set; }
    public CashEntryDirection Direction { get; private set; }
    public CashEntryReason Reason { get; private set; }
    public decimal Amount { get; private set; }
    public string? Description { get; private set; }
    public string? ReferenceId { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private CashEntry()
    {
        Direction = null!;
        Reason = null!;
    }

    public static CashEntry Create(
        Guid cashSessionId,
        CashEntryDirection direction,
        CashEntryReason reason,
        decimal amount,
        DateTime occurredAt,
        string? description = null,
        string? referenceId = null)
    {
        Guard.AgainstEmpty(cashSessionId, nameof(cashSessionId));
        Guard.AgainstNull(direction, nameof(direction));
        Guard.AgainstNull(reason, nameof(reason));
        Guard.Against(amount, a => a <= 0, "Cash entry amount must be positive.");
        Guard.AgainstDefault(occurredAt, nameof(occurredAt));

        return new CashEntry
        {
            Id = Guid.NewGuid(),
            CashSessionId = cashSessionId,
            Direction = direction,
            Reason = reason,
            Amount = amount,
            OccurredAt = occurredAt,
            Description = description?.Trim(),
            ReferenceId = referenceId
        };
    }
}