namespace RadiologyCenter.Inventory.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    public const string PurchaseOrderItemsRequired = "Inventory.PurchaseOrderItemsRequired";
    public const string PurchaseOrderDuplicateItems = "Inventory.PurchaseOrderDuplicateItems";
    public const string ReceiveItemsRequired = "Inventory.ReceiveItemsRequired";
    public const string ReceiveDuplicateItems = "Inventory.ReceiveDuplicateItems";
    public const string ExpiryDatePast = "Inventory.ExpiryDatePast";
    public const string ItemNotFound = "Inventory.ItemNotFound";
    public const string SupplierNotFound = "Inventory.SupplierNotFound";
    public const string PurchaseOrderNotFound = "Inventory.PurchaseOrderNotFound";
    public const string InsufficientStock = "Inventory.InsufficientStock";
    public const string InvalidReceipt = "Inventory.InvalidReceipt";
}
