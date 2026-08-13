using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Cash.Domain.Entities;
using RadiologyCenter.Cash.Domain.Enumerations;
using RadiologyCenter.Cash.Infrastructure.Persistence;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Localhost.Extensions;

public class PaymentCashEntryRecorder : IPaymentCashEntryRecorder
{
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;
    private readonly AuditSoftDeleteInterceptor _auditInterceptor;

    public PaymentCashEntryRecorder(ICurrentUser currentUser, IClock clock, AuditSoftDeleteInterceptor auditInterceptor)
    {
        _currentUser = currentUser;
        _clock = clock;
        _auditInterceptor = auditInterceptor;
    }

    public async Task<Result> RecordAsync(
        Guid examinationId,
        decimal amount,
        string? description,
        IUnitOfWorkTransaction transaction,
        CancellationToken ct) =>
        await RecordEntryAsync(
            examinationId,
            amount,
            description,
            CashEntryDirection.In,
            CashEntryReason.Payment,
            transaction,
            ct);

    public async Task<Result> RecordRefundAsync(
        Guid examinationId,
        decimal amount,
        string? description,
        IUnitOfWorkTransaction transaction,
        CancellationToken ct) =>
        await RecordEntryAsync(
            examinationId,
            amount,
            description,
            CashEntryDirection.Out,
            CashEntryReason.Refund,
            transaction,
            ct);

    private async Task<Result> RecordEntryAsync(
        Guid examinationId,
        decimal amount,
        string? description,
        CashEntryDirection direction,
        CashEntryReason reason,
        IUnitOfWorkTransaction transaction,
        CancellationToken ct)
    {
        if (!Guid.TryParse(_currentUser.Id, out var userId))
            return Result.Failure(Error.Unauthorized());

        if (transaction.DbTransaction is null)
            return Result.Failure(Error.Failure("No active database transaction is available for the cash entry."));

        var dbTransaction = transaction.DbTransaction;

        var options = new DbContextOptionsBuilder<CashDbContext>()
            .UseSqlServer(dbTransaction.Connection!)
            .AddInterceptors(_auditInterceptor)
            .Options;

        await using var cashContext = new CashDbContext(options);
        await cashContext.Database.UseTransactionAsync(dbTransaction, ct);

        var session = await cashContext.CashSessions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == CashSessionStatus.Open, ct);

        if (session is null)
        {
            session = CashSession.Open(userId, 0, _clock.UtcNow);
            await cashContext.CashSessions.AddAsync(session, ct);
        }

        var entry = CashEntry.Create(
            session.Id,
            direction,
            reason,
            amount,
            _clock.UtcNow,
            description,
            examinationId.ToString());

        await cashContext.CashEntries.AddAsync(entry, ct);
        await cashContext.SaveChangesAsync(ct);

        return Result.Success();
    }
}