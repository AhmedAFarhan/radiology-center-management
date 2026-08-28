using System.ComponentModel.DataAnnotations;

namespace RadiologyCenter.Desktop.Features.Notifications.Models;

internal sealed class NotificationTemplateFormModel
{
    [Required(ErrorMessage = "validation.codeRequired")]
    [MaxLength(100, ErrorMessage = "validation.codeMaxLength100")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.nameRequired")]
    [MaxLength(200, ErrorMessage = "validation.nameMaxLength200")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(400, ErrorMessage = "validation.subjectMaxLength")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "validation.bodyRequired")]
    public string Body { get; set; } = string.Empty;
}

internal sealed class SendNotificationFormModel
{
    [Required(ErrorMessage = "validation.recipientRequired")]
    [MaxLength(500, ErrorMessage = "validation.recipientMaxLength")]
    public string Recipient { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.channelRequired")]
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
