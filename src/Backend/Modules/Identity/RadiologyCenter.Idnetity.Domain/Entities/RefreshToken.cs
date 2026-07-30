using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Idnetity.Domain.Entities;

public sealed class RefreshToken
{
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc is not null;

    internal RefreshToken(string token, DateTime expiresAtUtc)
    {
        Token = Guard.AgainstNullOrWhiteSpace(token, nameof(token));
        ExpiresAtUtc = Guard.AgainstDefault(expiresAtUtc, nameof(expiresAtUtc));
        CreatedAtUtc = DateTime.UtcNow;
    }

    private RefreshToken() { }

    public void Revoke()
    {
        if (IsRevoked) return;
        RevokedAtUtc = DateTime.UtcNow;
    }
}
