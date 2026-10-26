using System.ComponentModel.DataAnnotations;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Examinations.Models;

internal sealed class TypeFormModel
{
    [Required(ErrorMessage = "validation.nameRequired")]
    [MaxLength(200, ErrorMessage = "validation.nameMaxLength200")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.modalityRequired")]
    public string Modality { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.bodyPartRequired")]
    [MaxLength(200, ErrorMessage = "validation.bodyPartMaxLength")]
    public string BodyPart { get; set; } = string.Empty;

    public int StandardDurationMinutes { get; set; }
    public decimal Price { get; set; }
    public bool RequiresPreparation { get; set; }
    public bool RequiresConsent { get; set; }
    public List<TypeItemModel> Items { get; set; } = new();
}

internal sealed class TypeItemModel
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsContrast { get; set; }
    public bool IsRequired { get; set; }
    public string? Notes { get; set; }
}

internal sealed class ScheduleExamFormModel
{
    [Required(ErrorMessage = "validation.patientRequired")]
    public PatientDto? Patient { get; set; }

    [Required(ErrorMessage = "validation.examinationTypeRequired")]
    public ExaminationTypeDto? ExaminationType { get; set; }

    [Required(ErrorMessage = "validation.equipmentRequired")]
    public string? EquipmentId { get; set; }

    [Required(ErrorMessage = "validation.scheduledDateRequired")]
    public DateTime? ScheduledDate { get; set; }

    [Required(ErrorMessage = "validation.scheduledTimeRequired")]
    public TimeSpan? ScheduledTime { get; set; }

    public string Priority { get; set; } = "Routine";

    [MaxLength(1000, ErrorMessage = "validation.clinicalIndicationMaxLength")]
    public string? ClinicalIndication { get; set; }

    [MaxLength(500, ErrorMessage = "validation.notesMaxLength500")]
    public string? Notes { get; set; }
}
