using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Examinations.Models;

internal sealed class TypeFormModel
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Modality is required.")]
    public string Modality { get; set; } = string.Empty;

    [Required(ErrorMessage = "Body part is required.")]
    [MaxLength(200, ErrorMessage = "Body part must be 200 characters or fewer.")]
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
    [Required(ErrorMessage = "Scheduled date is required.")]
    public DateTime? ScheduledDate { get; set; }

    [Required(ErrorMessage = "Scheduled time is required.")]
    public TimeSpan? ScheduledTime { get; set; }
}
