using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Notifications.Models;

internal sealed class NotificationTemplateFormModel
{
    [Required(ErrorMessage = "Code is required.")]
    [MaxLength(100, ErrorMessage = "Code must be 100 characters or fewer.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(400, ErrorMessage = "Subject must be 400 characters or fewer.")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Body is required.")]
    public string Body { get; set; } = string.Empty;
}

internal sealed class SendNotificationFormModel
{
    [Required(ErrorMessage = "Recipient is required.")]
    [MaxLength(500, ErrorMessage = "Recipient must be 500 characters or fewer.")]
    public string Recipient { get; set; } = string.Empty;

    [Required(ErrorMessage = "Channel is required.")]
    public string Channel { get; set; } = "Sms";

    public string? TemplateCode { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? ReferenceId { get; set; }
    public string? Placeholders { get; set; }

    public Dictionary<string, string> PlaceholdersSplit()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(Placeholders))
            return result;

        foreach (var pair in Placeholders.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var eq = pair.IndexOf('=');
            if (eq <= 0)
                continue;

            result[pair[..eq].Trim()] = pair[(eq + 1)..].Trim();
        }

        return result;
    }
}
