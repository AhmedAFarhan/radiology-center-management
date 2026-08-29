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

    public const string TemplateIdRequired = "Notification.TemplateIdRequired";
    public const string RecipientRequired = "Notification.RecipientRequired";
    public const string RecipientTooLong = "Notification.RecipientTooLong";
    public const string ChannelRequired = "Notification.ChannelRequired";
    public const string ChannelTooLong = "Notification.ChannelTooLong";
    public const string TemplateCodeTooLong = "Notification.TemplateCodeTooLong";
    public const string SubjectTooLong = "Notification.SubjectTooLong";
    public const string ReferenceIdTooLong = "Notification.ReferenceIdTooLong";
    public const string TemplateCodeRequired = "Notification.TemplateCodeRequired";
    public const string TemplateNameRequired = "Notification.TemplateNameRequired";
    public const string TemplateNameTooLong = "Notification.TemplateNameTooLong";
    public const string TemplateSubjectRequired = "Notification.TemplateSubjectRequired";
    public const string TemplateSubjectTooLong = "Notification.TemplateSubjectTooLong";
    public const string TemplateBodyRequired = "Notification.TemplateBodyRequired";
}
