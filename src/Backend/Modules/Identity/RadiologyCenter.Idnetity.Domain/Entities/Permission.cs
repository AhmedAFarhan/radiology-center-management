using System.Security.Cryptography;
using System.Text;
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

    public Permission(Guid id, string code, string name, string? description = null, string? group = null)
        : base(id)
    {
        Code = Guard.AgainstNullOrWhiteSpace(code, nameof(code));
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Description = description;
        Group = group;
    }

    internal static Guid CreateDeterministicId(string code)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(code));
        return new Guid(hash);
    }
}
