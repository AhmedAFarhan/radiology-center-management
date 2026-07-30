using Microsoft.AspNetCore.Identity;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.BuildingBlocks.Domain.Events;
using RadiologyCenter.Idnetity.Domain.Events;

namespace RadiologyCenter.Idnetity.Domain.Entities;

public sealed class Role : IdentityRole<Guid>, IAggregateRoot
{
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Permission> _permissions = [];
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
    private void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    private Role() { }

    public static Role Create(string name, string? description = null, bool isSystem = false)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            NormalizedName = name.ToUpperInvariant(),
            Description = description,
            IsSystem = isSystem,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        role.RaiseDomainEvent(new RoleCreatedEvent(role.Id, name));
        return role;
    }

    public void Update(string name, string? description)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
    }

    public void AddPermission(Permission permission)
    {
        Guard.AgainstNull(permission, nameof(permission));
        if (_permissions.Any(p => p.Id == permission.Id)) return;

        _permissions.Add(permission);
        RaiseDomainEvent(new RolePermissionsUpdatedEvent(Id, _permissions.Select(p => p.Code).ToList()));
    }

    public void RemovePermission(Permission permission)
    {
        Guard.AgainstNull(permission, nameof(permission));
        if (_permissions.RemoveAll(p => p.Id == permission.Id) == 0) return;

        RaiseDomainEvent(new RolePermissionsUpdatedEvent(Id, _permissions.Select(p => p.Code).ToList()));
    }

    public void ClearPermissions()
    {
        if (_permissions.Count == 0) return;
        _permissions.Clear();
        RaiseDomainEvent(new RolePermissionsUpdatedEvent(Id, []));
    }

    public bool HasPermission(string permissionCode)
    {
        Guard.AgainstNullOrWhiteSpace(permissionCode, nameof(permissionCode));
        return _permissions.Any(p =>
            p.Code.Equals(permissionCode, StringComparison.OrdinalIgnoreCase));
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
    }
}
