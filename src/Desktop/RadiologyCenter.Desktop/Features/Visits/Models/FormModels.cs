using System.ComponentModel.DataAnnotations;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Visits.Models;

internal sealed class VisitFormModel
{
    public string? RadiologistId { get; set; }

    public string? TechnicianId { get; set; }

    [Required(ErrorMessage = "validation.patientRequired")]
    public PatientDto? Patient { get; set; }

    [Required(ErrorMessage = "validation.examinationTypeRequired")]
    public ExaminationTypeDto? ExaminationType { get; set; }

    [Required(ErrorMessage = "validation.clinicalIndicationRequired")]
    [MaxLength(1000, ErrorMessage = "validation.clinicalIndicationMaxLength")]
    public string ClinicalIndication { get; set; } = string.Empty;

    public string Priority { get; set; } = "Routine";
    public string Status { get; set; } = "Scheduled";

    public DateTime? ScheduledDate { get; set; }

    public TimeSpan? ScheduledTime { get; set; }

    public decimal Discount { get; set; }
    public bool IsDiscountPercentage { get; set; }
    public decimal Paid { get; set; }
    public string? Notes { get; set; }
}

internal sealed class VisitScheduleModel
{
    [Required(ErrorMessage = "validation.scheduledDateRequired")]
    public DateTime? ScheduledDate { get; set; }

    [Required(ErrorMessage = "validation.scheduledTimeRequired")]
    public TimeSpan? ScheduledTime { get; set; }
}
