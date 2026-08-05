namespace RadiologyCenter.Examinations.Application.DTOs;

public record FinancialAnalyticsDto(
    int ExamCount,
    decimal TotalCollected,
    decimal TotalBilled,
    decimal TotalDiscounts,
    decimal Receivables,
    decimal AvgPerExam,
    IReadOnlyList<RevenuePointDto> RevenueByMonth,
    IReadOnlyList<RevenueByModalityDto> RevenueByModality,
    IReadOnlyList<ReceivableBucketDto> ReceivableAging);

public record RevenuePointDto(
    string Month,
    decimal Collected,
    decimal Billed);

public record RevenueByModalityDto(
    string Modality,
    decimal Collected,
    int ExamCount);

public record ReceivableBucketDto(
    string Bucket,
    decimal Amount,
    int ExamCount);

public record FinancialExamRowDto(
    Guid Id,
    string ExaminationTypeName,
    DateTime? CompletedAt,
    decimal Billed,
    decimal Discount,
    decimal Paid,
    decimal Remaining);

/// <summary>
/// Lightweight projection of the billing fields of a completed examination, used by the analytics read side.
/// </summary>
public record ExamFinancialProjection(
    Guid Id,
    Guid ExaminationTypeId,
    DateTime? CompletedAt,
    decimal Price,
    decimal Discount,
    bool IsDiscountPercentage,
    decimal Paid,
    decimal Remaining);
