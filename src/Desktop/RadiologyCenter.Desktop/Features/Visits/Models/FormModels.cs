using System.ComponentModel.DataAnnotations;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Visits.Models;

internal sealed class VisitFormModel
{
    public string? RadiologistId { get; set; }

    public string? TechnicianId { get; set; }

    [Required(ErrorMessage = "Patient is required.")]
    public PatientDto? Patient { get; set; }

    [Required(ErrorMessage = "Examination type is required.")]
    public ExaminationTypeDto? ExaminationType { get; set; }

    [Required(ErrorMessage = "Clinical indication is required.")]
    [MaxLength(1000, ErrorMessage = "Clinical indication must be 1000 characters or fewer.")]
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
    [Required(ErrorMessage = "Scheduled date is required.")]
    public DateTime? ScheduledDate { get; set; }

    [Required(ErrorMessage = "Scheduled time is required.")]
    public TimeSpan? ScheduledTime { get; set; }
}
