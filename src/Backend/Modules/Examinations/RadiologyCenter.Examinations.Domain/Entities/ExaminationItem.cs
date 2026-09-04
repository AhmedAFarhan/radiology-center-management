using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Errors;

namespace RadiologyCenter.Examinations.Domain.Entities;

public sealed class ExaminationItem : Entity<Guid>
{
    public Guid ExaminationId { get; private set; }
    public Guid ItemId { get; private set; }
    public int Quantity { get; private set; }
    public bool IsContrast { get; private set; }
    public bool IsRequired { get; private set; }
    public string? Notes { get; private set; }
    public decimal UnitCost { get; private set; }

    private ExaminationItem() { }

    public static ExaminationItem Create(
        Guid examinationId,
        Guid itemId,
        int quantity,
        bool isContrast = false,
        bool isRequired = false,
        string? notes = null,
        decimal unitCost = 0)
    {
        Guard.AgainstEmpty(examinationId, nameof(examinationId));
        Guard.AgainstEmpty(itemId, nameof(itemId));
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));
        Guard.Against(unitCost, c => c < 0, DomainErrors.UnitCostNegative, "Unit cost cannot be negative.");

        return new ExaminationItem
        {
            Id = Guid.NewGuid(),
            ExaminationId = examinationId,
            ItemId = itemId,
            Quantity = quantity,
            IsContrast = isContrast,
            IsRequired = isRequired,
            Notes = notes?.Trim(),
            UnitCost = unitCost
        };
    }
}
