namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IBookingResourceDirectory
{
    Task<bool> IsOperationalEquipmentAsync(Guid equipmentId, CancellationToken ct);
    Task<Result> ValidateAsync(Examination examination, ExaminationTypeInfo type, DateTime? startUtc, DateTime? endUtc, CancellationToken ct);
}
