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

    // ── Item validation ──────────────────────────────────────────────
    public const string ItemIdRequired = "Inventory.ItemIdRequired";
    public const string ItemNameRequired = "Inventory.ItemNameRequired";
    public const string ItemNameTooLong = "Inventory.ItemNameTooLong";
    public const string ItemCategoryRequired = "Inventory.ItemCategoryRequired";
    public const string ItemUnitRequired = "Inventory.ItemUnitRequired";
    public const string ItemBrandTooLong = "Inventory.ItemBrandTooLong";
    public const string ReorderLevelCannotBeNegative = "Inventory.ReorderLevelCannotBeNegative";
    public const string ReorderQuantityCannotBeNegative = "Inventory.ReorderQuantityCannotBeNegative";
    public const string StorageInstructionsTooLong = "Inventory.StorageInstructionsTooLong";

    // ── Purchase order validation ────────────────────────────────────
    public const string PurchaseOrderIdRequired = "Inventory.PurchaseOrderIdRequired";
    public const string PurchaseOrderLineItemIdRequired = "Inventory.PurchaseOrderLineItemIdRequired";
    public const string PurchaseOrderQuantityMustBePositive = "Inventory.PurchaseOrderQuantityMustBePositive";
    public const string PurchaseOrderUnitCostCannotBeNegative = "Inventory.PurchaseOrderUnitCostCannotBeNegative";
    public const string PurchaseOrderNotesTooLong = "Inventory.PurchaseOrderNotesTooLong";

    // ── Supplier validation ──────────────────────────────────────────
    public const string SupplierIdRequired = "Inventory.SupplierIdRequired";
    public const string SupplierNameRequired = "Inventory.SupplierNameRequired";
    public const string SupplierNameTooLong = "Inventory.SupplierNameTooLong";
    public const string SupplierPhoneRequired = "Inventory.SupplierPhoneRequired";
    public const string SupplierPhoneTooLong = "Inventory.SupplierPhoneTooLong";
    public const string SupplierContactPersonTooLong = "Inventory.SupplierContactPersonTooLong";
    public const string SupplierEmailInvalid = "Inventory.SupplierEmailInvalid";
    public const string SupplierAddressTooLong = "Inventory.SupplierAddressTooLong";
    public const string SupplierTaxNumberTooLong = "Inventory.SupplierTaxNumberTooLong";
    public const string SupplierPaymentTermsTooLong = "Inventory.SupplierPaymentTermsTooLong";

    // ── Stock validation ─────────────────────────────────────────────
    public const string StockQuantityMustBePositive = "Inventory.StockQuantityMustBePositive";
    public const string StockReferenceTooLong = "Inventory.StockReferenceTooLong";
    public const string StockNotesTooLong = "Inventory.StockNotesTooLong";

    // ── Receive line validation ──────────────────────────────────────
    public const string ReceiveLineItemIdRequired = "Inventory.ReceiveLineItemIdRequired";
    public const string ReceiveLineQuantityMustBePositive = "Inventory.ReceiveLineQuantityMustBePositive";
    public const string ReceiveLineLotNumberRequired = "Inventory.ReceiveLineLotNumberRequired";
    public const string ReceiveLineLotNumberTooLong = "Inventory.ReceiveLineLotNumberTooLong";
}
