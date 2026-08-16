using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Cash.Application.Localization;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Application.DTOs;
using RadiologyCenter.Cash.Domain.Entities;
using RadiologyCenter.Cash.Domain.Enumerations;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.CloseCashSession;

public static class CloseCashSessionCommandHandler
{
    public static async Task<Result<CashHandoverDto>> HandleAsync(
        CloseCashSessionCommand command,
        ICurrentUser currentUser,
        ICashSessionRepository sessionRepository,
        ICashEntryRepository entryRepository,
        ICashHandoverRepository handoverRepository,
        ICashUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.Id, out var closingUserId))
            return Result.Failure<CashHandoverDto>(Error.Unauthorized());

        var session = await sessionRepository.GetByIdAsync(command.CashSessionId, ct);
        if (session is null)
            return Result.Failure<CashHandoverDto>(Error.NotFound(ErrorCodes.SessionNotFound, "CashSession", command.CashSessionId));
        if (session.UserId != closingUserId)
            return Result.Failure<CashHandoverDto>(Error.Forbidden(ErrorCodes.CloseNotOwnSession, "You can only close your own cash session."));
        if (session.Status != CashSessionStatus.Open)
            return Result.Failure<CashHandoverDto>(Error.Conflict(ErrorCodes.CloseSessionNotOpen, "Cannot close a session that is not open."));

        var expected = session.OpeningFloat;
        var movements = await entryRepository.GetSessionMovementsAsync(new[] { session.Id }, ct);
        if (movements.TryGetValue(session.Id, out var movement))
            expected += movement.Movement;

        var now = DateTime.UtcNow;
        var handover = CashHandover.Create(session.Id, expected, command.CountedTotal, now, closingUserId, command.Notes);

        session.Close(now);

        if (command.ReceivingUserId is { } receivingUserId && receivingUserId != Guid.Empty)
        {
            if (await sessionRepository.GetOpenSessionByUserAsync(receivingUserId, ct) is not null)
                return Result.Failure<CashHandoverDto>(Error.Conflict(ErrorCodes.ReceiverAlreadyOpenSession, "The receiving user already has an open session."));

            var successor = CashSession.Open(
                receivingUserId,
                command.ReceivingOpeningFloat ?? command.CountedTotal,
                now,
                notes: "Opened on handover.");

            await sessionRepository.AddAsync(successor, ct);
            handover.SetReceivingSession(successor.Id);
        }

        await handoverRepository.AddAsync(handover, ct);
        sessionRepository.Update(session);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(handover.ToDto());
    }
}