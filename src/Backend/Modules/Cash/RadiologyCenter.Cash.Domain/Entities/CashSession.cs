using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Cash.Domain.Enumerations;
using RadiologyCenter.Cash.Domain.Errors;
using RadiologyCenter.Cash.Domain.Events;

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
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

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
        Guard.Against(openingFloat, f => f < 0, DomainErrors.OpeningFloatNegative, "Opening float cannot be negative.");
        Guard.AgainstDefault(openedAt, nameof(openedAt));

        var session = new CashSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WorkShiftId = workShiftId,
            Status = CashSessionStatus.Open,
            OpeningFloat = openingFloat,
            OpenedAt = openedAt,
            Notes = notes?.Trim()
        };

        session.RaiseDomainEvent(new CashSessionOpenedEvent(session.Id, userId, openingFloat));
        return session;
    }

    public void Close(DateTime closedAt)
    {
        Guard.Against(Status, s => s != CashSessionStatus.Open, DomainErrors.CloseSessionNotOpen, "Cannot close a session that is not open.");
        Status = CashSessionStatus.Closed;
        ClosedAt = closedAt;
        RaiseDomainEvent(new CashSessionClosedEvent(Id, UserId, closedAt));
    }
}