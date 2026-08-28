using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.ReadingRoom.Models;

internal sealed class AssignStaffFormModel
{
    [Required(ErrorMessage = "validation.radiologistRequired")]
    public string? RadiologistId { get; set; }

    [Required(ErrorMessage = "validation.technicianRequired")]
    public string? TechnicianId { get; set; }
}
