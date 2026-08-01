using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.Examinations.Domain.Entities;

public sealed class ExaminationTypeItem : Entity<Guid>
{
    public Guid ExaminationTypeId { get; private set; }
    public Guid ItemId { get; private set; }
    public int Quantity { get; private set; }
    public bool IsContrast { get; private set; }
    public bool IsRequired { get; private set; }
    public string? Notes { get; private set; }

    private ExaminationTypeItem() { }

    public static ExaminationTypeItem Create(
        Guid examinationTypeId,
        Guid itemId,
        int quantity,
        bool isContrast = false,
        bool isRequired = false,
        string? notes = null)
    {
        Guard.AgainstEmpty(examinationTypeId, nameof(examinationTypeId));
        Guard.AgainstEmpty(itemId, nameof(itemId));
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));

        return new ExaminationTypeItem
        {
            Id = Guid.NewGuid(),
            ExaminationTypeId = examinationTypeId,
            ItemId = itemId,
            Quantity = quantity,
            IsContrast = isContrast,
            IsRequired = isRequired,
            Notes = notes?.Trim()
        };
    }

    public void Update(Guid itemId, int quantity, bool isContrast, bool isRequired, string? notes = null)
    {
        Guard.AgainstEmpty(itemId, nameof(itemId));
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));

        ItemId = itemId;
        Quantity = quantity;
        IsContrast = isContrast;
        IsRequired = isRequired;
        Notes = notes?.Trim();
    }
}
