using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Cash.Models;

internal sealed class OpenSessionFormModel
{
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "validation.openingFloatInvalid")]
    public decimal OpeningFloat { get; set; }

    [MaxLength(500, ErrorMessage = "validation.notesMaxLength500")]
    public string? Notes { get; set; }
}

internal sealed class CloseSessionFormModel
{
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "validation.countedTotalInvalid")]
    public decimal CountedTotal { get; set; }

    [MaxLength(100, ErrorMessage = "validation.receivingUserIdMaxLength")]
    public string? ReceivingUserId { get; set; }

    [Range(0, (double)decimal.MaxValue, ErrorMessage = "validation.receivingOpeningFloatInvalid")]
    public decimal? ReceivingOpeningFloat { get; set; }

    [MaxLength(1000, ErrorMessage = "validation.notesMaxLength1000")]
    public string? Notes { get; set; }
}

internal sealed class AddEntryFormModel
{
    public string Direction { get; set; } = "In";

    public string Reason { get; set; } = "Payment";

    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "validation.amountGreaterThanZero")]
    public decimal Amount { get; set; }

    [MaxLength(100, ErrorMessage = "validation.referenceMaxLength")]
    public string? ReferenceId { get; set; }

    [MaxLength(500, ErrorMessage = "validation.descriptionMaxLength")]
    public string? Description { get; set; }
}
