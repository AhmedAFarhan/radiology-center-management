using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.BuildingBlocks.Domain.Entities;

public abstract class TranslationBase : Entity<Guid>
{
    public string Language { get; protected set; }
    public string Name { get; protected set; }

    protected TranslationBase(string language, string name)
    {
        Language = Guard.AgainstNullOrWhiteSpace(language, nameof(language));
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
    }

    protected TranslationBase() { }
}