using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.Examinations.Domain.Entities;

public sealed class ExaminationHistoryItem : Entity<Guid>
{
    public Guid ExaminationHistoryId { get; private set; }
    public Guid ItemId { get; private set; }
    public string ItemName { get; private set; }
    public int ItemCategory { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public bool IsContrast { get; private set; }
    public bool IsRequired { get; private set; }
    public string? Notes { get; private set; }

    private ExaminationHistoryItem()
    {
        ItemName = null!;
    }

    public static ExaminationHistoryItem Create(
        Guid examinationHistoryId,
        Guid itemId,
        string itemName,
        int itemCategory,
        int quantity,
        decimal unitCost = 0,
        bool isContrast = false,
        bool isRequired = false,
        string? notes = null)
    {
        Guard.AgainstEmpty(examinationHistoryId, nameof(examinationHistoryId));
        Guard.AgainstEmpty(itemId, nameof(itemId));
        Guard.AgainstNullOrWhiteSpace(itemName, nameof(itemName));
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));
        Guard.Against(unitCost, c => c < 0, "Unit cost cannot be negative.");

        return new ExaminationHistoryItem
        {
            Id = Guid.NewGuid(),
            ExaminationHistoryId = examinationHistoryId,
            ItemId = itemId,
            ItemName = itemName.Trim(),
            ItemCategory = itemCategory,
            Quantity = quantity,
            UnitCost = unitCost,
            IsContrast = isContrast,
            IsRequired = isRequired,
            Notes = notes?.Trim()
        };
    }
}
