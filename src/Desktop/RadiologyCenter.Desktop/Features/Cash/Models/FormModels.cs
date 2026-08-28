using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Cash.Models;

internal sealed class OpenSessionFormModel
{
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Opening float must be a valid amount.")]
    public decimal OpeningFloat { get; set; }

    [MaxLength(500, ErrorMessage = "Notes must be 500 characters or fewer.")]
    public string? Notes { get; set; }
}

internal sealed class CloseSessionFormModel
{
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Counted total must be a valid amount.")]
    public decimal CountedTotal { get; set; }

    [MaxLength(100, ErrorMessage = "Receiving user ID must be 100 characters or fewer.")]
    public string? ReceivingUserId { get; set; }

    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Receiving opening float must be a valid amount.")]
    public decimal? ReceivingOpeningFloat { get; set; }

    [MaxLength(1000, ErrorMessage = "Notes must be 1000 characters or fewer.")]
    public string? Notes { get; set; }
}

internal sealed class AddEntryFormModel
{
    public string Direction { get; set; } = "In";

    public string Reason { get; set; } = "Payment";

    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [MaxLength(100, ErrorMessage = "Reference must be 100 characters or fewer.")]
    public string? ReferenceId { get; set; }

    [MaxLength(500, ErrorMessage = "Description must be 500 characters or fewer.")]
    public string? Description { get; set; }
}
