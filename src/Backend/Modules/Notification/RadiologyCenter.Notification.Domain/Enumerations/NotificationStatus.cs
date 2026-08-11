using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Notification.Domain.Enumerations;

public sealed class NotificationStatus : Enumeration
{
    public static readonly NotificationStatus Pending = new(1, "Pending");
    public static readonly NotificationStatus Sent = new(2, "Sent");
    public static readonly NotificationStatus Failed = new(3, "Failed");

    private NotificationStatus(int value, string name) : base(value, name) { }
}