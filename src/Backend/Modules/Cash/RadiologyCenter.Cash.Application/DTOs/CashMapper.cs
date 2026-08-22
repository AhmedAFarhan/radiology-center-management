using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.Cash.Domain.Entities;

namespace RadiologyCenter.Cash.Application.DTOs;

internal static class CashMapper
{
    public static CashEntryDto ToDto(this CashEntry entry) =>
        new(
            entry.Id,
            entry.CashSessionId,
            entry.Direction.LocalizedName(),
            entry.Reason.LocalizedName(),
            entry.Amount,
            entry.Description,
            entry.ReferenceId,
            entry.OccurredAt,
            entry.Direction.Name,
            entry.Reason.Name);

    public static CashHandoverDto ToDto(this CashHandover handover, string closedByName = "") =>
        new(
            handover.Id,
            handover.CashSessionId,
            handover.ExpectedTotal,
            handover.CountedTotal,
            handover.OverShortAmount,
            handover.ClosedAt,
            handover.ClosedByUserId,
            closedByName,
            handover.ApprovedByUserId,
            handover.ApprovedAt,
            handover.ReceivingCashSessionId,
            handover.Notes);

    public static CashSessionDto ToDto(this CashSession session, decimal balance, string userName, int entryCount) =>
        new(
            session.Id,
            session.UserId,
            userName,
            session.WorkShiftId,
            session.Status.LocalizedName(),
            session.OpeningFloat,
            balance,
            session.OpenedAt,
            session.ClosedAt,
            session.Notes,
            entryCount,
            session.Status.Name);
}