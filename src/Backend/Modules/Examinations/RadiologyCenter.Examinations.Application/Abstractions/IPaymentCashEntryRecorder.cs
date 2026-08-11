namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IPaymentCashEntryRecorder
{
    Task<Result> RecordAsync(Guid examinationId, decimal amount, string? description, CancellationToken ct = default);
}