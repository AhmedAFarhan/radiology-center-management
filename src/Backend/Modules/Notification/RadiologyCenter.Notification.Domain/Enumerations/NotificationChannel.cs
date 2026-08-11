using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Notification.Domain.Enumerations;

public sealed class NotificationChannel : Enumeration
{
    public static readonly NotificationChannel Sms = new(1, "Sms");
    public static readonly NotificationChannel Email = new(2, "Email");
    public static readonly NotificationChannel Push = new(3, "Push");

    private NotificationChannel(int value, string name) : base(value, name) { }
}