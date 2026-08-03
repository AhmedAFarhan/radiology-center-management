namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IExaminationFeeResolver
{
    Task<ExaminationFeeResolution?> ResolveAsync(
        Guid examinationTypeId,
        decimal examinationTypePrice,
        Guid radiologistId,
        Guid technicianId,
        Guid? referralDoctorId,
        CancellationToken ct);
}