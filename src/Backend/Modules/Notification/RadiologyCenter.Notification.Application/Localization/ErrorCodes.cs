namespace RadiologyCenter.Notification.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    public const string TemplateCodeOrBodyRequired = "Notification.TemplateCodeOrBodyRequired";
    public const string TemplateNotFound = "Notification.TemplateNotFound";
    public const string InvalidChannel = "Notification.InvalidChannel";
    public const string InactiveTemplate = "Notification.InactiveTemplate";
    public const string TemplateCodeExists = "Notification.TemplateCodeExists";
    public const string SendFailed = "Notification.SendFailed";
    public const string DeliveryFailed = "Notification.DeliveryFailed";
}
