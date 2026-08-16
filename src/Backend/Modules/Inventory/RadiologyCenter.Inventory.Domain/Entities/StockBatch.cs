using RadiologyCenter.BuildingBlocks.Domain.Auditing;
using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.Inventory.Domain.Errors;

namespace RadiologyCenter.Inventory.Domain.Entities;

public sealed class StockBatch : AuditableEntity<Guid>
{
    public Guid ItemId { get; private set; }
    public string LotNumber { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public int QuantityReceived { get; private set; }
    public int QuantityRemaining { get; private set; }
    public Guid? SupplierId { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    private StockBatch()
    {
        LotNumber = null!;
    }

    public static StockBatch Create(
        Guid itemId,
        string lotNumber,
        int quantityReceived,
        DateTime? expiryDate = null,
        Guid? supplierId = null)
    {
        Guard.AgainstEmpty(itemId, nameof(itemId));
        Guard.AgainstNullOrWhiteSpace(lotNumber, nameof(lotNumber));
        Guard.AgainstNegativeOrZero(quantityReceived, nameof(quantityReceived));
        Guard.Against(expiryDate, e => e.HasValue && e.Value.Date < DateTime.UtcNow.Date, DomainErrors.BatchExpiryDatePast, "Batch expiry date cannot be in the past.");

        return new StockBatch
        {
            Id = Guid.NewGuid(),
            ItemId = itemId,
            LotNumber = lotNumber.Trim(),
            QuantityReceived = quantityReceived,
            QuantityRemaining = quantityReceived,
            ExpiryDate = expiryDate,
            SupplierId = supplierId
        };
    }

    public void Issue(int quantity)
    {
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));
        Guard.Against(quantity, q => q > QuantityRemaining, DomainErrors.InsufficientBatchStock, $"Cannot issue {quantity} from batch '{LotNumber}'; only {QuantityRemaining} remaining.");

        QuantityRemaining -= quantity;
    }

    public void IncreaseRemaining(int quantity)
    {
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));

        QuantityRemaining += quantity;
    }

    public bool IsExpired(DateTime asOf) =>
        ExpiryDate.HasValue && ExpiryDate.Value.Date < asOf.Date;
}
