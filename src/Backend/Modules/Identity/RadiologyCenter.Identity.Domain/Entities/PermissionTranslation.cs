using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.Identity.Domain.Entities;

public sealed class PermissionTranslation : TranslationBase
{
    public Guid PermissionId { get; private set; }
    public string? Description { get; private set; }
    public string? Group { get; private set; }

    private PermissionTranslation() { }

    public static PermissionTranslation Create(
        Guid permissionId,
        string language,
        string name,
        string? description = null,
        string? group = null)
    {
        Guard.AgainstEmpty(permissionId, nameof(permissionId));

        return new PermissionTranslation
        {
            Id = Guid.NewGuid(),
            PermissionId = permissionId,
            Language = Guard.AgainstNullOrWhiteSpace(language, nameof(language)),
            Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name)),
            Description = description,
            Group = group
        };
    }

    public void Update(string name, string? description = null, string? group = null)
    {
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Description = description;
        Group = group;
    }
}