using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.Examinations.Domain.Entities;

public sealed class ExaminationItem : Entity<Guid>
{
    public Guid ExaminationId { get; private set; }
    public Guid ItemId { get; private set; }
    public int Quantity { get; private set; }
    public bool IsContrast { get; private set; }
    public bool IsRequired { get; private set; }
    public string? Notes { get; private set; }

    private ExaminationItem() { }

    public static ExaminationItem Create(
        Guid examinationId,
        Guid itemId,
        int quantity,
        bool isContrast = false,
        bool isRequired = false,
        string? notes = null)
    {
        Guard.AgainstEmpty(examinationId, nameof(examinationId));
        Guard.AgainstEmpty(itemId, nameof(itemId));
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));

        return new ExaminationItem
        {
            Id = Guid.NewGuid(),
            ExaminationId = examinationId,
            ItemId = itemId,
            Quantity = quantity,
            IsContrast = isContrast,
            IsRequired = isRequired,
            Notes = notes?.Trim()
        };
    }
}
