using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Cash.Application.DTOs;
using RadiologyCenter.Cash.Domain.Enumerations;

namespace RadiologyCenter.Cash.Application.Commands.Sessions.ApproveCashHandover;

public static class ApproveCashHandoverCommandHandler
{
    public static async Task<Result<CashHandoverDto>> HandleAsync(
        ApproveCashHandoverCommand command,
        ICurrentUser currentUser,
        ICashSessionRepository sessionRepository,
        ICashHandoverRepository handoverRepository,
        ICashUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.Id, out var approvingUserId))
            return Result.Failure<CashHandoverDto>(Error.Unauthorized());

        var session = await sessionRepository.GetByIdAsync(command.CashSessionId, ct);
        if (session is null)
            return Result.Failure<CashHandoverDto>(Error.NotFound("CashSession", command.CashSessionId));
        if (session.Status != CashSessionStatus.Closed)
            return Result.Failure<CashHandoverDto>(Error.Conflict("Only closed sessions can have their handover approved."));

        var handover = await handoverRepository.GetBySessionAsync(command.CashSessionId, ct);
        if (handover is null)
            return Result.Failure<CashHandoverDto>(Error.NotFound("CashHandover", command.CashSessionId));
        if (handover.ApprovedAt is not null)
            return Result.Failure<CashHandoverDto>(Error.Conflict("This handover is already approved."));

        handover.Approve(approvingUserId, DateTime.UtcNow);
        handoverRepository.Update(handover);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(handover.ToDto());
    }
}