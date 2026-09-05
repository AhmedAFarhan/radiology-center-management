using Microsoft.AspNetCore.Identity;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.BuildingBlocks.Domain.Events;
using RadiologyCenter.Identity.Domain.Events;

namespace RadiologyCenter.Identity.Domain.Entities;

public sealed class User : IdentityUser<Guid>, IAggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string TimeZoneId { get; private set; }
    public bool IsActive { get; private set; }
    public bool MustChangePassword { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Role> _assignedRoles = [];
    public IReadOnlyCollection<Role> AssignedRoles => _assignedRoles.AsReadOnly();

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private readonly List<UserSession> _sessions = [];
    public IReadOnlyCollection<UserSession> Sessions => _sessions.AsReadOnly();

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
    private void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    private User()
    {
        FirstName = null!;
        LastName = null!;
        TimeZoneId = "Africa/Cairo";
    }

    public static User Create(string userName, string email, string firstName, string lastName, string phoneNumber, string timeZoneId = "Africa/Cairo")
    {
        Guard.AgainstNullOrWhiteSpace(userName, nameof(userName));
        Guard.AgainstNullOrWhiteSpace(email, nameof(email));
        Guard.AgainstNullOrWhiteSpace(firstName, nameof(firstName));
        Guard.AgainstNullOrWhiteSpace(lastName, nameof(lastName));
        Guard.AgainstNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
        Guard.AgainstNullOrWhiteSpace(timeZoneId, nameof(timeZoneId));

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            NormalizedUserName = userName.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            TimeZoneId = timeZoneId,
            IsActive = true,
            LockoutEnabled = true,
            SecurityStamp = Guid.NewGuid().ToString("D"),
            ConcurrencyStamp = Guid.NewGuid().ToString("D"),
            CreatedAt = DateTime.UtcNow
        };

        // user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, userName, email));
        return user;
    }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
    {
        Guard.AgainstNullOrWhiteSpace(firstName, nameof(firstName));
        Guard.AgainstNullOrWhiteSpace(lastName, nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    public void UpdateTimeZone(string timeZoneId)
    {
        Guard.AgainstNullOrWhiteSpace(timeZoneId, nameof(timeZoneId));
        TimeZoneId = timeZoneId;
    }

    public void ConfirmEmail() => EmailConfirmed = true;

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = Guard.AgainstNullOrWhiteSpace(passwordHash, nameof(passwordHash));
        SecurityStamp = Guid.NewGuid().ToString("D");
    }

    public void RequirePasswordChange() => MustChangePassword = true;

    public void PasswordChanged()
    {
        MustChangePassword = false;
        SecurityStamp = Guid.NewGuid().ToString("D");
    }

    public void AssignRole(Role role)
    {
        Guard.AgainstNull(role, nameof(role));
        if (_assignedRoles.Any(r => r.Id == role.Id)) return;

        _assignedRoles.Add(role);
        // RaiseDomainEvent(new UserRolesUpdatedEvent(Id, _assignedRoles.Select(r => r.Id).ToList()));
    }

    public void RemoveRole(Role role)
    {
        Guard.AgainstNull(role, nameof(role));
        if (_assignedRoles.RemoveAll(r => r.Id == role.Id) == 0) return;

        RaiseDomainEvent(new UserRolesUpdatedEvent(Id, _assignedRoles.Select(r => r.Id).ToList()));
    }

    public void UpdateRoles(IEnumerable<Role> roles)
    {
        Guard.AgainstNull(roles, nameof(roles));
        var targetRoles = roles.DistinctBy(r => r.Id).ToList();

        var added = targetRoles.Where(r => !_assignedRoles.Any(ar => ar.Id == r.Id)).ToList();
        var removed = _assignedRoles.Where(ar => targetRoles.All(r => r.Id != ar.Id)).ToList();

        if (added.Count == 0 && removed.Count == 0) return;

        foreach (var role in removed)
            _assignedRoles.Remove(role);

        foreach (var role in added)
            _assignedRoles.Add(role);

        RaiseDomainEvent(new UserRolesUpdatedEvent(Id, _assignedRoles.Select(r => r.Id).ToList()));
    }

    public bool HasRole(Guid roleId) => _assignedRoles.Any(r => r.Id == roleId);

    public bool HasPermission(string permissionCode)
    {
        Guard.AgainstNullOrWhiteSpace(permissionCode, nameof(permissionCode));
        return _assignedRoles.Any(r => r.HasPermission(permissionCode));
    }

    public IReadOnlyCollection<string> GetEffectivePermissions() =>
        _assignedRoles.SelectMany(r => r.Permissions).Select(p => p.Code).Distinct().ToList();

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        RaiseDomainEvent(new UserReactivatedEvent(Id));
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        RaiseDomainEvent(new UserDeactivatedEvent(Id));
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        AccessFailedCount = 0;
        RaiseDomainEvent(new UserLoggedInEvent(Id));
    }

    public void IncrementAccessFailedCount() => AccessFailedCount++;

    public void Lock(DateTimeOffset lockoutEnd) => LockoutEnd = lockoutEnd;

    public void Unlock()
    {
        LockoutEnd = null;
        AccessFailedCount = 0;
    }

    public void SetTwoFactorEnabled(bool enabled) => TwoFactorEnabled = enabled;

    public bool IsLockedOut => LockoutEnd is not null && LockoutEnd > DateTimeOffset.UtcNow;

    public void RegisterFailedLoginAttempt(int maxAttempts, TimeSpan lockoutDuration)
    {
        IncrementAccessFailedCount();
        if (LockoutEnabled && AccessFailedCount >= maxAttempts)
            Lock(DateTimeOffset.UtcNow.Add(lockoutDuration));
    }

    public RefreshToken AddRefreshToken(string token, DateTime expiresAtUtc)
    {
        var refreshToken = new RefreshToken(token, expiresAtUtc);
        _refreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public void RevokeRefreshToken(string token) =>
        _refreshTokens.FirstOrDefault(rt => rt.Token == token)?.Revoke();

    public void RevokeAllRefreshTokens()
    {
        foreach (var rt in _refreshTokens)
            rt.Revoke();
    }

    public bool HasValidRefreshToken(string token) =>
        _refreshTokens.Any(rt => rt.Token == token && !rt.IsExpired && !rt.IsRevoked);

    public UserSession StartSession(string refreshToken)
    {
        var session = new UserSession(refreshToken);
        _sessions.Add(session);
        return session;
    }

    public void RevokeSession(string refreshToken) =>
        _sessions.FirstOrDefault(s => s.RefreshToken == refreshToken)?.Revoke();

    public void RevokeAllSessions()
    {
        foreach (var session in _sessions)
            session.Revoke();
    }

    public bool HasActiveSession(string refreshToken) =>
        _sessions.Any(s => s.RefreshToken == refreshToken && s.IsActive);

    public void RecordSessionActivity(string refreshToken) =>
        _sessions.FirstOrDefault(s => s.RefreshToken == refreshToken)?.RecordActivity();
}
