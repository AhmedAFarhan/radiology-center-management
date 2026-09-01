namespace RadiologyCenter.Notification.Domain.Errors;

/// <summary>
/// Stable semantic codes for domain-invariant violations. Thrown as
/// <see cref="DomainException"/> codes and resolved through the "codes"
/// section of the module JSON resource files.
/// </summary>
public static class DomainErrors
{
    public const string RecipientRequired = "Notification.RecipientRequired";
    public const string SubjectRequired = "Notification.SubjectRequired";
    public const string BodyRequired = "Notification.BodyRequired";
    public const string TemplateCodeRequired = "Notification.TemplateCodeRequired";
    public const string InvalidChannel = "Notification.InvalidChannel";
    public const string MaxAttemptsExceeded = "Notification.MaxAttemptsExceeded";
}
