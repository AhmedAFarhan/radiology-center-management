using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.Examinations.Domain.Entities;

namespace RadiologyCenter.Examinations.Application.DTOs;

internal static class ExaminationMapper
{
    public static ExaminationDto ToDto(this Examination examination, string examinationTypeName) =>
        new(
            examination.Id,
            examination.PatientId,
            examination.ExaminationTypeId,
            examinationTypeName,
            examination.ReferralDoctorId,
            examination.RadiologistId,
            examination.TechnicianId,
            examination.EquipmentId,
            examination.ClinicalIndication,
            examination.Priority.LocalizedName(),
            examination.Priority.Name,
            examination.Status.LocalizedName(),
            examination.Status.Name,
            examination.ScheduledAt,
            examination.ScheduledEnd,
            examination.StartedAt,
            examination.CompletedAt,
            examination.PerformedByUserId,
            examination.Notes,
            examination.CancellationReason,
            examination.Price,
            examination.Discount,
            examination.IsDiscountPercentage,
            examination.Paid,
            examination.Remaining,
            examination.StudyInstanceUID,
            examination.AccessionNumber,
            examination.ImagesReceivedAt,
            examination.Items.Select(i => i.Adapt<ExaminationItemDto>()).ToList());
}
