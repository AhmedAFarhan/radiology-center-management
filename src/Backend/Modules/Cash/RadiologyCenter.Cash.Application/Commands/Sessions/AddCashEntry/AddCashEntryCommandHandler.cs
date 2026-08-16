using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Cash.Application.Localization;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Application.Commands.Sessions.Common;
using RadiologyCenter.Cash.Application.DTOs;
using RadiologyCenter.Cash.Domain.Enumerations;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.AddCashEntry;

public static class AddCashEntryCommandHandler
{
    public static async Task<Result<CashEntryDto>> HandleAsync(
        AddCashEntryCommand command,
        ICurrentUser currentUser,
        ICashSessionRepository sessionRepository,
        ICashEntryRepository entryRepository,
        ICashUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.Id, out var userId))
            return Result.Failure<CashEntryDto>(Error.Unauthorized());

        var session = await sessionRepository.GetByIdAsync(command.CashSessionId, ct);
        if (session is null)
            return Result.Failure<CashEntryDto>(Error.NotFound(ErrorCodes.SessionNotFound, "CashSession", command.CashSessionId));
        if (session.UserId != userId)
            return Result.Failure<CashEntryDto>(Error.Forbidden(ErrorCodes.EntryNotOwnSession, "You can only add entries to your own cash session."));
        if (session.Status != CashSessionStatus.Open)
            return Result.Failure<CashEntryDto>(Error.Conflict(ErrorCodes.AddEntryToClosedSession, "Cannot add entries to a closed session."));

        var direction = CashEntryDirection.FromName<CashEntryDirection>(command.Direction.ToString());
        var reason = CashEntryReason.FromName<CashEntryReason>(command.Reason.ToString());

        var entry = CashEntry.Create(
            command.CashSessionId,
            direction,
            reason,
            command.Amount,
            DateTime.UtcNow,
            command.Description,
            command.ReferenceId);

        await entryRepository.AddAsync(entry, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(entry.ToDto());
    }
}