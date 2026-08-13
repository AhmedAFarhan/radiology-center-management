namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IPaymentCashEntryRecorder
{
    Task<Result> RecordAsync(
        Guid examinationId,
        decimal amount,
        string? description,
        IUnitOfWorkTransaction transaction,
        CancellationToken ct = default);

    Task<Result> RecordRefundAsync(
        Guid examinationId,
        decimal amount,
        string? description,
        IUnitOfWorkTransaction transaction,
        CancellationToken ct = default);
}