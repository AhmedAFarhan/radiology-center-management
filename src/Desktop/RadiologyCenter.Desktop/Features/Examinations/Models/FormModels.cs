using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "validation.scheduledDateRequired")]
    public DateTime? ScheduledDate { get; set; }

    [Required(ErrorMessage = "validation.scheduledTimeRequired")]
    public TimeSpan? ScheduledTime { get; set; }
}
