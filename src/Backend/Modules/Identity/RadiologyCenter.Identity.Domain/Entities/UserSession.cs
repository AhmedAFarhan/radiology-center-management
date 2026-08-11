using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Identity.Domain.Entities;

public sealed class UserSession
{
    public Guid Id { get; private set; }

    public string RefreshToken { get; private set; } = null!;

    public DateTime StartedAtUtc { get; private set; }

    public DateTime LastActivityAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public bool IsActive => RevokedAtUtc is null;

    internal UserSession(string refreshToken)
    {
        Id = Guid.NewGuid();
        RefreshToken = Guard.AgainstNullOrWhiteSpace(refreshToken, nameof(refreshToken));
        StartedAtUtc = DateTime.UtcNow;
        LastActivityAtUtc = DateTime.UtcNow;
    }

    private UserSession() { }

    public void RecordActivity()
    {
        if (!IsActive) return;
        LastActivityAtUtc = DateTime.UtcNow;
    }

    public void Revoke()
    {
        if (!IsActive) return;
        RevokedAtUtc = DateTime.UtcNow;
    }
}