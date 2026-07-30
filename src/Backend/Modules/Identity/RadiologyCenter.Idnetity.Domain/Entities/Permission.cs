using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.Idnetity.Domain.Entities;

public sealed class Permission : Entity<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Group { get; private set; }

    private Permission() => (Code, Name) = (null!, null!);

    public Permission(string code, string name, string? description = null, string? group = null)
        : base(Guid.NewGuid())
    {
        Code = Guard.AgainstNullOrWhiteSpace(code, nameof(code));
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Description = description;
        Group = group;
    }

    public void Update(string name, string? description, string? group)
    {
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Description = description;
        Group = group;
    }
}
