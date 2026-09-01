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

public sealed record InsuranceAnalyticsDto(
    int TotalClaims,
    int DraftClaims,
    int SubmittedClaims,
    int ApprovedClaims,
    int RejectedClaims,
    int PaidClaims,
    decimal TotalBilledAmount,
    decimal TotalPayerShare,
    decimal TotalPatientShare,
    decimal TotalSettled,
    decimal OutstandingAmount,
    decimal ApprovalRate,
    IReadOnlyList<InsuranceClaimRowDto> ClaimRows);

public sealed record InsuranceClaimRowDto(
    Guid ClaimId,
    string PatientName,
    string InsuranceCompany,
    string PolicyNumber,
    decimal BilledAmount,
    decimal PayerShare,
    decimal PatientShare,
    string Status,
    DateTime? SubmittedAt,
    DateTime? ApprovedAt,
    decimal SettledAmount,
    decimal RemainingOwed);

public sealed record CashFlowReportDto(
    decimal TotalInflows,
    decimal TotalOutflows,
    decimal NetCashFlow,
    int TotalSessions,
    int TotalEntries,
    decimal AvgSessionBalance,
    IReadOnlyList<CashFlowPeriodDto> ByMonth,
    IReadOnlyList<CashFlowEntryTypeDto> ByReason,
    IReadOnlyList<CashFlowSessionSummaryDto> SessionSummaries);

public sealed record CashFlowPeriodDto(
    string Month,
    decimal Inflows,
    decimal Outflows,
    decimal Net);

public sealed record CashFlowEntryTypeDto(
    string Reason,
    decimal InflowAmount,
    decimal OutflowAmount,
    int EntryCount);

public sealed record CashFlowSessionSummaryDto(
    string SessionId,
    string UserName,
    decimal OpeningFloat,
    decimal Balance,
    int EntryCount,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    string Status);