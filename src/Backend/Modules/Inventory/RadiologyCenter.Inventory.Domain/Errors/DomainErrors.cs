namespace RadiologyCenter.Inventory.Domain.Errors;

/// <summary>
/// Stable semantic codes for domain-invariant violations. Thrown as
/// <see cref="DomainException"/> codes and resolved through the "codes"
/// section of the module JSON resource files.
/// </summary>
public static class DomainErrors
{
    public const string ReorderLevelNegative = "Inventory.ReorderLevelNegative";
    public const string ReorderQuantityNegative = "Inventory.ReorderQuantityNegative";
    public const string DuplicateItem = "Inventory.DuplicateItem";
    public const string ItemNotOnPurchaseOrder = "Inventory.ItemNotOnPurchaseOrder";
    public const string PurchaseOrderItemsRequired = "Inventory.PurchaseOrderItemsRequired";
    public const string PurchaseOrderCannotCancel = "Inventory.PurchaseOrderCannotCancel";
    public const string ReceiptsOrderedOnly = "Inventory.ReceiptsOrderedOnly";
    public const string PurchaseOrderNotDraft = "Inventory.PurchaseOrderNotDraft";
    public const string UnitCostNegative = "Inventory.UnitCostNegative";
    public const string ReceiptExceedsRemaining = "Inventory.ReceiptExceedsRemaining";
    public const string BatchExpiryDatePast = "Inventory.BatchExpiryDatePast";
    public const string InsufficientBatchStock = "Inventory.InsufficientBatchStock";
    public const string MovementQuantityZero = "Inventory.MovementQuantityZero";
}
