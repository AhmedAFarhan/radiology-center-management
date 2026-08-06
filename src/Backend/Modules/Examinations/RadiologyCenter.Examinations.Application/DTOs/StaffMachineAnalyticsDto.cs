namespace RadiologyCenter.Examinations.Application.DTOs;

public record StaffMachineAnalyticsDto(
    IReadOnlyList<StaffPerformanceDto> Radiologists,
    IReadOnlyList<StaffPerformanceDto> Technicians,
    IReadOnlyList<ReferralDoctorPerformanceDto> ReferralDoctors,
    IReadOnlyList<ModalityUtilizationDto> ModalityUtilization);

public record StaffPerformanceDto(
    Guid StaffId,
    string Name,
    int CompletedExams,
    decimal FeeIncome);

public record ReferralDoctorPerformanceDto(
    Guid ReferralDoctorId,
    string Name,
    int ReferredExams,
    decimal ReferralFeeIncome);

public record ModalityUtilizationDto(
    string Modality,
    int CompletedExams,
    int ActiveMachines,
    decimal ExamsPerMachine);
