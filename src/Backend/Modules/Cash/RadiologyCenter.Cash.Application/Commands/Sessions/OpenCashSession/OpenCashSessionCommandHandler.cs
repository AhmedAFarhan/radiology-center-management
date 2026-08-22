using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Cash.Application.Localization;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Application.DTOs;
using RadiologyCenter.Cash.Domain.Entities;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.OpenCashSession;

public static class OpenCashSessionCommandHandler
{
    public static async Task<Result<CashSessionDto>> HandleAsync(
        OpenCashSessionCommand command,
        ICurrentUser currentUser,
        ICashSessionRepository sessionRepository,
        ICashUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.Id, out var userId))
            return Result.Failure<CashSessionDto>(Error.Unauthorized());

        if (await sessionRepository.GetOpenSessionByUserAsync(userId, ct) is not null)
            return Result.Failure<CashSessionDto>(Error.Conflict(ErrorCodes.SessionAlreadyOpen, "A cash session is already open for this user."));

        var session = CashSession.Open(
            userId,
            command.OpeningFloat,
            DateTime.UtcNow,
            command.WorkShiftId,
            command.Notes);

        await sessionRepository.AddAsync(session, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new CashSessionDto(
            session.Id,
            session.UserId,
            currentUser.Name ?? string.Empty,
            session.WorkShiftId,
            session.Status.Name,
            session.OpeningFloat,
            session.OpeningFloat,
            session.OpenedAt,
            null,
            session.Notes,
            0,
            session.Status.Name));
    }
}