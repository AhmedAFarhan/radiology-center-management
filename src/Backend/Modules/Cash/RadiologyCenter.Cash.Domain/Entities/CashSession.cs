using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Cash.Domain.Enumerations;

namespace RadiologyCenter.Cash.Domain.Entities;

public sealed class CashSession : SoftDeletableAggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public Guid? WorkShiftId { get; private set; }
    public CashSessionStatus Status { get; private set; }
    public decimal OpeningFloat { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? Notes { get; private set; }

    private CashSession()
    {
        Status = null!;
    }

    public static CashSession Open(
        Guid userId,
        decimal openingFloat,
        DateTime openedAt,
        Guid? workShiftId = null,
        string? notes = null)
    {
        Guard.AgainstEmpty(userId, nameof(userId));
        Guard.Against(openingFloat, f => f < 0, "Opening float cannot be negative.");
        Guard.AgainstDefault(openedAt, nameof(openedAt));

        return new CashSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WorkShiftId = workShiftId,
            Status = CashSessionStatus.Open,
            OpeningFloat = openingFloat,
            OpenedAt = openedAt,
            Notes = notes?.Trim()
        };
    }

    public void Close(DateTime closedAt)
    {
        Guard.Against(Status, s => s != CashSessionStatus.Open, "Cannot close a session that is not open.");
        Status = CashSessionStatus.Closed;
        ClosedAt = closedAt;
    }
}