using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.Notification.Domain.Enumerations;

namespace RadiologyCenter.Notification.Domain.Entities;

public sealed class NotificationTemplate : Entity<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public bool IsActive { get; private set; }

    private NotificationTemplate()
    {
        Code = null!;
        Name = null!;
        Subject = null!;
        Body = null!;
    }

    public NotificationTemplate(string code, string name, string subject, string body)
    {
        Id = Guid.NewGuid();
        Code = code.Trim();
        Name = name.Trim();
        Subject = subject.Trim();
        Body = body.Trim();
        IsActive = true;
    }

    public void Update(string name, string subject, string body)
    {
        Name = name.Trim();
        Subject = subject.Trim();
        Body = body.Trim();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}