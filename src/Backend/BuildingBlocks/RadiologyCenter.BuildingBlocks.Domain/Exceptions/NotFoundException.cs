using RadiologyCenter.BuildingBlocks.Domain.Localization;

namespace RadiologyCenter.BuildingBlocks.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public string EntityName { get; }
    public object Key { get; }

    public NotFoundException(string entityName, object key)
        : base(MessageCodes.Shared.KeyWasNotFound, $"{entityName} with key '{key}' was not found.")
    {
        EntityName = entityName;
        Key = key;
    }
}