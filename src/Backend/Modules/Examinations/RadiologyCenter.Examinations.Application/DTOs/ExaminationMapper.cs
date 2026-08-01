using Mapster;
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
            examination.ReferringDoctor,
            examination.ClinicalIndication,
            examination.Priority.Name,
            examination.Status.Name,
            examination.ScheduledAt,
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
            examination.Items.Select(i => i.Adapt<ExaminationItemDto>()).ToList());
}
