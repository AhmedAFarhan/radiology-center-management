using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.ReadingRoom.Models;

internal sealed class AssignStaffFormModel
{
    [Required(ErrorMessage = "Radiologist is required.")]
    public string? RadiologistId { get; set; }

    [Required(ErrorMessage = "Technician is required.")]
    public string? TechnicianId { get; set; }
}
