using RadiologyCenter.Identity.Domain.Entities;

namespace RadiologyCenter.Identity.Domain;

public static class Permissions
{
    private static Permission Create(string code, string name, string? description = null, string? group = null) => new(Permission.CreateDeterministicId(code), code, name, description, group);

    public const string PatientsCreateCode  = "patients.create";
    public const string PatientsReadCode    = "patients.read";
    public const string PatientsUpdateCode  = "patients.update";
    public const string PatientsDeleteCode  = "patients.delete";
    public static readonly Permission PatientsCreate  = Create(PatientsCreateCode, "Create Patients",  "Create new patient records",     "Patients");
    public static readonly Permission PatientsRead    = Create(PatientsReadCode,   "Read Patients",    "View patient records",           "Patients");
    public static readonly Permission PatientsUpdate  = Create(PatientsUpdateCode, "Update Patients",  "Modify existing patient records", "Patients");
    public static readonly Permission PatientsDelete  = Create(PatientsDeleteCode, "Delete Patients",  "Remove patient records",          "Patients");

    public const string AppointmentsCreateCode  = "appointments.create";
    public const string AppointmentsReadCode    = "appointments.read";
    public const string AppointmentsUpdateCode  = "appointments.update";
    public const string AppointmentsDeleteCode  = "appointments.delete";
    public const string AppointmentsConfirmCode = "appointments.confirm";
    public static readonly Permission AppointmentsCreate  = Create(AppointmentsCreateCode, "Create Appointments",  "Schedule new appointments",            "Appointments");
    public static readonly Permission AppointmentsRead    = Create(AppointmentsReadCode,   "Read Appointments",    "View appointment details",             "Appointments");
    public static readonly Permission AppointmentsUpdate  = Create(AppointmentsUpdateCode, "Update Appointments",  "Modify existing appointments",         "Appointments");
    public static readonly Permission AppointmentsDelete  = Create(AppointmentsDeleteCode, "Delete Appointments",  "Cancel or remove appointments",        "Appointments");
    public static readonly Permission AppointmentsConfirm = Create(AppointmentsConfirmCode,"Confirm Appointments", "Confirm scheduled appointments",       "Appointments");

    public const string ReportsCreateCode  = "reports.create";
    public const string ReportsReadCode    = "reports.read";
    public const string ReportsUpdateCode  = "reports.update";
    public const string ReportsDeleteCode  = "reports.delete";
    public const string ReportsExportCode  = "reports.export";
    public static readonly Permission ReportsCreate  = Create(ReportsCreateCode, "Create Reports",  "Generate new radiology reports",  "Reports");
    public static readonly Permission ReportsRead    = Create(ReportsReadCode,   "Read Reports",    "View radiology reports",          "Reports");
    public static readonly Permission ReportsUpdate  = Create(ReportsUpdateCode, "Update Reports",  "Modify existing reports",         "Reports");
    public static readonly Permission ReportsDelete  = Create(ReportsDeleteCode, "Delete Reports",  "Remove reports",                  "Reports");
    public static readonly Permission ReportsExport  = Create(ReportsExportCode, "Export Reports",  "Export reports to external formats", "Reports");

    public const string UsersCreateCode      = "users.create";
    public const string UsersReadCode        = "users.read";
    public const string UsersUpdateCode      = "users.update";
    public const string UsersDeleteCode      = "users.delete";
    public const string UsersManageRolesCode = "users.manage-roles";
    public static readonly Permission UsersCreate      = Create(UsersCreateCode,      "Create Users",        "Create new user accounts",              "Identity");
    public static readonly Permission UsersRead        = Create(UsersReadCode,        "Read Users",          "View user accounts",                    "Identity");
    public static readonly Permission UsersUpdate      = Create(UsersUpdateCode,      "Update Users",        "Modify user accounts",                  "Identity");
    public static readonly Permission UsersDelete      = Create(UsersDeleteCode,      "Delete Users",        "Remove user accounts",                  "Identity");
    public static readonly Permission UsersManageRoles = Create(UsersManageRolesCode, "Manage User Roles",   "Assign or remove roles from users",     "Identity");

    public const string RolesCreateCode          = "roles.create";
    public const string RolesReadCode            = "roles.read";
    public const string RolesUpdateCode          = "roles.update";
    public const string RolesDeleteCode          = "roles.delete";
    public const string RolesManagePermissionsCode = "roles.manage-permissions";
    public static readonly Permission RolesCreate          = Create(RolesCreateCode,          "Create Roles",            "Create new roles",                        "Identity");
    public static readonly Permission RolesRead            = Create(RolesReadCode,            "Read Roles",              "View roles",                              "Identity");
    public static readonly Permission RolesUpdate          = Create(RolesUpdateCode,          "Update Roles",            "Modify role details",                     "Identity");
    public static readonly Permission RolesDelete          = Create(RolesDeleteCode,          "Delete Roles",            "Remove roles",                            "Identity");
    public static readonly Permission RolesManagePermissions = Create(RolesManagePermissionsCode, "Manage Role Permissions", "Assign or remove permissions from roles", "Identity");

    public const string SettingsReadCode   = "settings.read";
    public const string SettingsUpdateCode = "settings.update";
    public static readonly Permission SettingsRead   = Create(SettingsReadCode,   "Read Settings",   "View system settings",   "Settings");
    public static readonly Permission SettingsUpdate = Create(SettingsUpdateCode, "Update Settings", "Modify system settings", "Settings");

    public const string InventoryItemsCreateCode  = "inventory.items.create";
    public const string InventoryItemsReadCode    = "inventory.items.read";
    public const string InventoryItemsUpdateCode  = "inventory.items.update";
    public const string InventoryItemsDeleteCode  = "inventory.items.delete";
    public static readonly Permission InventoryItemsCreate  = Create(InventoryItemsCreateCode,  "Create Inventory Items",  "Add new inventory items",         "Inventory");
    public static readonly Permission InventoryItemsRead    = Create(InventoryItemsReadCode,    "Read Inventory Items",    "View inventory items",            "Inventory");
    public static readonly Permission InventoryItemsUpdate  = Create(InventoryItemsUpdateCode,  "Update Inventory Items",  "Modify inventory items",          "Inventory");
    public static readonly Permission InventoryItemsDelete  = Create(InventoryItemsDeleteCode,  "Delete Inventory Items",  "Remove inventory items",          "Inventory");

    public const string InventorySuppliersCreateCode  = "inventory.suppliers.create";
    public const string InventorySuppliersReadCode    = "inventory.suppliers.read";
    public const string InventorySuppliersUpdateCode  = "inventory.suppliers.update";
    public const string InventorySuppliersDeleteCode  = "inventory.suppliers.delete";
    public static readonly Permission InventorySuppliersCreate  = Create(InventorySuppliersCreateCode,  "Create Inventory Suppliers",  "Add new suppliers",   "Inventory");
    public static readonly Permission InventorySuppliersRead    = Create(InventorySuppliersReadCode,    "Read Inventory Suppliers",    "View suppliers",      "Inventory");
    public static readonly Permission InventorySuppliersUpdate  = Create(InventorySuppliersUpdateCode,  "Update Inventory Suppliers",  "Modify suppliers",    "Inventory");
    public static readonly Permission InventorySuppliersDelete  = Create(InventorySuppliersDeleteCode,  "Delete Inventory Suppliers",  "Remove suppliers",    "Inventory");

    public const string InventoryPurchaseOrdersCreateCode  = "inventory.purchase-orders.create";
    public const string InventoryPurchaseOrdersReadCode    = "inventory.purchase-orders.read";
    public const string InventoryPurchaseOrdersUpdateCode  = "inventory.purchase-orders.update";
    public const string InventoryPurchaseOrdersDeleteCode  = "inventory.purchase-orders.delete";
    public static readonly Permission InventoryPurchaseOrdersCreate  = Create(InventoryPurchaseOrdersCreateCode,  "Create Purchase Orders",  "Create purchase orders",   "Inventory");
    public static readonly Permission InventoryPurchaseOrdersRead    = Create(InventoryPurchaseOrdersReadCode,    "Read Purchase Orders",    "View purchase orders",     "Inventory");
    public static readonly Permission InventoryPurchaseOrdersUpdate  = Create(InventoryPurchaseOrdersUpdateCode,  "Update Purchase Orders",  "Modify purchase orders",   "Inventory");
    public static readonly Permission InventoryPurchaseOrdersDelete  = Create(InventoryPurchaseOrdersDeleteCode,  "Delete Purchase Orders",  "Remove purchase orders",   "Inventory");

    public const string InventoryStockReadCode  = "inventory.stock.read";
    public const string InventoryStockIssueCode = "inventory.stock.issue";
    public static readonly Permission InventoryStockRead  = Create(InventoryStockReadCode,  "Read Stock",     "View stock levels and movements", "Inventory");
    public static readonly Permission InventoryStockIssue = Create(InventoryStockIssueCode, "Issue Stock",    "Issue stock to patients",         "Inventory");

    private static readonly List<Permission> _all =
    [
        PatientsCreate, PatientsRead, PatientsUpdate, PatientsDelete,
        AppointmentsCreate, AppointmentsRead, AppointmentsUpdate, AppointmentsDelete, AppointmentsConfirm,
        ReportsCreate, ReportsRead, ReportsUpdate, ReportsDelete, ReportsExport,
        UsersCreate, UsersRead, UsersUpdate, UsersDelete, UsersManageRoles,
        RolesCreate, RolesRead, RolesUpdate, RolesDelete, RolesManagePermissions,
        SettingsRead, SettingsUpdate,
        InventoryItemsCreate, InventoryItemsRead, InventoryItemsUpdate, InventoryItemsDelete,
        InventorySuppliersCreate, InventorySuppliersRead, InventorySuppliersUpdate, InventorySuppliersDelete,
        InventoryPurchaseOrdersCreate, InventoryPurchaseOrdersRead, InventoryPurchaseOrdersUpdate, InventoryPurchaseOrdersDelete,
        InventoryStockRead, InventoryStockIssue
    ];

    public static IReadOnlyList<Permission> All => _all.AsReadOnly();

    public static Permission? GetByCode(string code) =>
        _all.FirstOrDefault(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

    public static bool IsValid(string code) =>
        _all.Any(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
}
