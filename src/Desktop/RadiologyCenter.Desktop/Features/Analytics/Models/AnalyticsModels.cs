namespace RadiologyCenter.Desktop.Features.Analytics.Models;

public sealed record FinancialAnalyticsDto(
    int ExamCount,
    decimal TotalCollected,
    decimal TotalBilled,
    decimal TotalDiscounts,
    decimal Receivables,
    decimal AvgPerExam,
    IReadOnlyList<RevenuePointDto> RevenueByMonth,
    IReadOnlyList<RevenueByModalityDto> RevenueByModality,
    IReadOnlyList<ReceivableBucketDto> ReceivableAging);

public sealed record RevenuePointDto(
    string Month,
    decimal Collected,
    decimal Billed);

public sealed record RevenueByModalityDto(
    string Modality,
    decimal Collected,
    int ExamCount);

public sealed record ReceivableBucketDto(
    string Bucket,
    decimal Amount,
    int ExamCount);

public sealed record FinancialExamRowDto(
    Guid Id,
    string ExaminationTypeName,
    DateTime? CompletedAt,
    decimal Billed,
    decimal Discount,
    decimal Paid,
    decimal Remaining);

public sealed record OperationalAnalyticsDto(
    int TotalExams,
    int CompletedExams,
    int CancelledExams,
    decimal CompletionRate,
    double AvgDurationMinutes,
    double AvgTimeToStartMinutes,
    IReadOnlyList<StatusCountDto> Funnel,
    IReadOnlyList<MonthlyVolumeDto> VolumeByMonth,
    IReadOnlyList<ModalityVolumeDto> VolumeByModality,
    IReadOnlyList<PriorityVolumeDto> VolumeByPriority);

public sealed record StatusCountDto(
    string Status,
    int Count);

public sealed record MonthlyVolumeDto(
    string Month,
    int Total,
    int Completed);

public sealed record ModalityVolumeDto(
    string Modality,
    int Total,
    int Completed);

public sealed record PriorityVolumeDto(
    string Priority,
    int Count);

public sealed record StaffMachineAnalyticsDto(
    IReadOnlyList<StaffPerformanceDto> Radiologists,
    IReadOnlyList<StaffPerformanceDto> Technicians,
    IReadOnlyList<ReferralDoctorPerformanceDto> ReferralDoctors,
    IReadOnlyList<ModalityUtilizationDto> ModalityUtilization);

public sealed record StaffPerformanceDto(
    Guid StaffId,
    string Name,
    int CompletedExams,
    decimal FeeIncome);

public sealed record ReferralDoctorPerformanceDto(
    Guid ReferralDoctorId,
    string Name,
    int ReferredExams,
    decimal ReferralFeeIncome);

public sealed record ModalityUtilizationDto(
    string Modality,
    int CompletedExams,
    int ActiveMachines,
    decimal ExamsPerMachine);

public sealed record ProfitAnalyticsDto(
    DateTime From,
    DateTime To,
    decimal RevenueCollected,
    decimal TotalBilled,
    decimal Discounts,
    decimal StaffCaseFees,
    decimal ReferralFees,
    decimal LaborCosts,
    bool LaborCostsTracked,
    decimal MaterialCosts,
    bool MaterialCostsTracked,
    decimal TotalCosts,
    decimal NetProfit,
    decimal NetMargin);