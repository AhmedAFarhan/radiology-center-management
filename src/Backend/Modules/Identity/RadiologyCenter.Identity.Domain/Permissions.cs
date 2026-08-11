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

    public const string AnalyticsReadCode = "analytics.read";
    public static readonly Permission AnalyticsRead = Create(AnalyticsReadCode, "Read Analytics", "View analytics and business intelligence dashboards", "Analytics");

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

    public const string ExaminationsTypesManageCode = "examinations.types.manage";
    public const string ExaminationsCreateCode     = "examinations.create";
    public const string ExaminationsReadCode       = "examinations.read";
    public const string ExaminationsUpdateCode     = "examinations.update";
    public const string ExaminationsDeleteCode     = "examinations.delete";
    public const string ExaminationsPerformCode    = "examinations.perform";
    public const string ExaminationsCancelCode     = "examinations.cancel";
    public static readonly Permission ExaminationsTypesManage = Create(ExaminationsTypesManageCode, "Manage Examination Types",  "Create, update, delete and activate examination types", "Examinations");
    public static readonly Permission ExaminationsCreate       = Create(ExaminationsCreateCode,     "Create Examinations",         "Create visits and add examinations",                  "Examinations");
    public static readonly Permission ExaminationsRead         = Create(ExaminationsReadCode,       "Read Examinations",           "View visits and examinations",                        "Examinations");
    public static readonly Permission ExaminationsUpdate       = Create(ExaminationsUpdateCode,     "Update Examinations",         "Modify examinations and their items",                 "Examinations");
    public static readonly Permission ExaminationsDelete       = Create(ExaminationsDeleteCode,     "Delete Examinations",         "Remove examinations",                                 "Examinations");
    public static readonly Permission ExaminationsPerform      = Create(ExaminationsPerformCode,    "Perform Examinations",        "Start and complete examinations",                     "Examinations");
    public static readonly Permission ExaminationsCancel       = Create(ExaminationsCancelCode,     "Cancel Examinations",         "Cancel visits and examinations",                      "Examinations");

    public const string StaffCreateCode      = "resources.staff.create";
    public const string StaffReadCode        = "resources.staff.read";
    public const string StaffUpdateCode      = "resources.staff.update";
    public const string StaffDeleteCode      = "resources.staff.delete";
    public static readonly Permission StaffCreate      = Create(StaffCreateCode,      "Create Staff",        "Add new staff members",             "Resources");
    public static readonly Permission StaffRead        = Create(StaffReadCode,        "Read Staff",          "View staff members",                "Resources");
    public static readonly Permission StaffUpdate      = Create(StaffUpdateCode,      "Update Staff",        "Modify staff members",              "Resources");
    public static readonly Permission StaffDelete      = Create(StaffDeleteCode,      "Delete Staff",        "Remove staff members",              "Resources");

    public const string EquipmentCreateCode = "resources.equipment.create";
    public const string EquipmentReadCode   = "resources.equipment.read";
    public const string EquipmentUpdateCode = "resources.equipment.update";
    public const string EquipmentDeleteCode = "resources.equipment.delete";
    public static readonly Permission EquipmentCreate = Create(EquipmentCreateCode, "Create Equipment",  "Add new equipment",                  "Resources");
    public static readonly Permission EquipmentRead   = Create(EquipmentReadCode,   "Read Equipment",    "View equipment",                     "Resources");
    public static readonly Permission EquipmentUpdate = Create(EquipmentUpdateCode, "Update Equipment",  "Modify equipment and its status",    "Resources");
    public static readonly Permission EquipmentDelete = Create(EquipmentDeleteCode, "Delete Equipment",  "Remove equipment",                   "Resources");

    public const string ShiftsCreateCode = "resources.shifts.create";
    public const string ShiftsReadCode   = "resources.shifts.read";
    public const string ShiftsUpdateCode = "resources.shifts.update";
    public const string ShiftsDeleteCode = "resources.shifts.delete";
    public static readonly Permission ShiftsCreate = Create(ShiftsCreateCode, "Create Work Shifts",  "Add new work shifts",   "Resources");
    public static readonly Permission ShiftsRead   = Create(ShiftsReadCode,   "Read Work Shifts",    "View work shifts",      "Resources");
    public static readonly Permission ShiftsUpdate = Create(ShiftsUpdateCode, "Update Work Shifts",  "Modify work shifts",    "Resources");
    public static readonly Permission ShiftsDelete = Create(ShiftsDeleteCode, "Delete Work Shifts",  "Remove work shifts",    "Resources");

    public const string LeaveCreateCode = "resources.leave.create";
    public const string LeaveReadCode   = "resources.leave.read";
    public const string LeaveUpdateCode = "resources.leave.update";
    public const string LeaveDeleteCode = "resources.leave.delete";
    public static readonly Permission LeaveCreate = Create(LeaveCreateCode, "Create Leave",  "Add new leave records",   "Resources");
    public static readonly Permission LeaveRead   = Create(LeaveReadCode,   "Read Leave",    "View leave records",      "Resources");
    public static readonly Permission LeaveUpdate = Create(LeaveUpdateCode, "Update Leave",  "Modify leave records",    "Resources");
    public static readonly Permission LeaveDelete = Create(LeaveDeleteCode, "Delete Leave",  "Remove leave records",    "Resources");

    public const string ReferralDoctorsCreateCode = "resources.referrals.create";
    public const string ReferralDoctorsReadCode   = "resources.referrals.read";
    public const string ReferralDoctorsUpdateCode = "resources.referrals.update";
    public const string ReferralDoctorsDeleteCode = "resources.referrals.delete";
    public static readonly Permission ReferralDoctorsCreate = Create(ReferralDoctorsCreateCode, "Create Referral Doctors",  "Add new referral doctors",   "Resources");
    public static readonly Permission ReferralDoctorsRead   = Create(ReferralDoctorsReadCode,   "Read Referral Doctors",    "View referral doctors",      "Resources");
    public static readonly Permission ReferralDoctorsUpdate = Create(ReferralDoctorsUpdateCode, "Update Referral Doctors",  "Modify referral doctors",    "Resources");
    public static readonly Permission ReferralDoctorsDelete = Create(ReferralDoctorsDeleteCode, "Delete Referral Doctors",  "Remove referral doctors",    "Resources");

    public const string PayrollReadCode                    = "payroll.read";
    public const string PayrollSalaryComponentsManageCode  = "payroll.salary-components.manage";
    public const string PayrollSalaryManageCode            = "payroll.salary.manage";
    public const string PayrollAllowancesManageCode        = "payroll.allowances.manage";
    public const string PayrollFeesManageCode              = "payroll.fees.manage";
    public const string PayrollPayRunsManageCode           = "payroll.payruns.manage";
    public const string PayrollPayRunsRunCode              = "payroll.payruns.run";
    public static readonly Permission PayrollRead                   = Create(PayrollReadCode,                   "Read Payroll",            "View payroll records",                           "Payroll");
    public static readonly Permission PayrollSalaryComponentsManage = Create(PayrollSalaryComponentsManageCode, "Manage Salary Components", "Create, update, activate and delete salary components", "Payroll");
    public static readonly Permission PayrollSalaryManage           = Create(PayrollSalaryManageCode,           "Manage Salaries",         "Create, update, activate and delete staff salaries",     "Payroll");
    public static readonly Permission PayrollAllowancesManage       = Create(PayrollAllowancesManageCode,       "Manage Allowances",       "Create, update, activate and delete allowance assignments", "Payroll");
    public static readonly Permission PayrollFeesManage             = Create(PayrollFeesManageCode,             "Manage Payroll Fees",     "Create, update, activate and delete examination and referral fees", "Payroll");
    public static readonly Permission PayrollPayRunsManage          = Create(PayrollPayRunsManageCode,          "Manage Pay Runs",         "Create pay runs and manage their payslips",       "Payroll");
    public static readonly Permission PayrollPayRunsRun             = Create(PayrollPayRunsRunCode,             "Run Pay Rolls",           "Compute, approve, reject and pay pay runs",       "Payroll");

    public const string InsuranceCompaniesCreateCode  = "insurance.companies.create";
    public const string InsuranceCompaniesReadCode    = "insurance.companies.read";
    public const string InsuranceCompaniesUpdateCode  = "insurance.companies.update";
    public const string InsuranceCompaniesDeleteCode  = "insurance.companies.delete";
    public static readonly Permission InsuranceCompaniesCreate  = Create(InsuranceCompaniesCreateCode,  "Create Insurance Companies",  "Add new insurance companies",       "Insurance");
    public static readonly Permission InsuranceCompaniesRead    = Create(InsuranceCompaniesReadCode,    "Read Insurance Companies",    "View insurance companies",          "Insurance");
    public static readonly Permission InsuranceCompaniesUpdate  = Create(InsuranceCompaniesUpdateCode,  "Update Insurance Companies",  "Modify insurance company details",  "Insurance");
    public static readonly Permission InsuranceCompaniesDelete  = Create(InsuranceCompaniesDeleteCode,  "Delete Insurance Companies",  "Remove insurance companies",        "Insurance");

    public const string InsurancePoliciesCreateCode  = "insurance.policies.create";
    public const string InsurancePoliciesReadCode    = "insurance.policies.read";
    public const string InsurancePoliciesUpdateCode  = "insurance.policies.update";
    public const string InsurancePoliciesDeleteCode  = "insurance.policies.delete";
    public static readonly Permission InsurancePoliciesCreate  = Create(InsurancePoliciesCreateCode,  "Create Insurance Policies",  "Add new patient insurance policies",  "Insurance");
    public static readonly Permission InsurancePoliciesRead    = Create(InsurancePoliciesReadCode,    "Read Insurance Policies",    "View patient insurance policies",     "Insurance");
    public static readonly Permission InsurancePoliciesUpdate  = Create(InsurancePoliciesUpdateCode,  "Update Insurance Policies",  "Modify policies, coverage and status", "Insurance");
    public static readonly Permission InsurancePoliciesDelete  = Create(InsurancePoliciesDeleteCode,  "Delete Insurance Policies",  "Remove insurance policies",           "Insurance");

    public const string InsurancePreAuthorizationsCreateCode  = "insurance.preauthorizations.create";
    public const string InsurancePreAuthorizationsReadCode    = "insurance.preauthorizations.read";
    public const string InsurancePreAuthorizationsUpdateCode  = "insurance.preauthorizations.update";
    public static readonly Permission InsurancePreAuthorizationsCreate  = Create(InsurancePreAuthorizationsCreateCode,  "Create Pre-authorizations",  "Request pre-authorization for examinations",  "Insurance");
    public static readonly Permission InsurancePreAuthorizationsRead    = Create(InsurancePreAuthorizationsReadCode,    "Read Pre-authorizations",    "View pre-authorizations and documents",        "Insurance");
    public static readonly Permission InsurancePreAuthorizationsUpdate  = Create(InsurancePreAuthorizationsUpdateCode,  "Update Pre-authorizations",  "Approve, deny or attach documents",            "Insurance");
    public const string InsurancePreAuthorizationsAttachDocumentCode = "insurance.preauthorizations.attach-document";
    public static readonly Permission InsurancePreAuthorizationsAttachDocument = Create(InsurancePreAuthorizationsAttachDocumentCode, "Attach Pre-authorization Documents", "Upload or remove official approval documents", "Insurance");

    public const string InsuranceClaimsCreateCode  = "insurance.claims.create";
    public const string InsuranceClaimsReadCode    = "insurance.claims.read";
    public const string InsuranceClaimsUpdateCode  = "insurance.claims.update";
    public const string InsuranceClaimsSettleCode  = "insurance.claims.settle";
    public static readonly Permission InsuranceClaimsCreate  = Create(InsuranceClaimsCreateCode,  "Create Claims",     "Create claims for covered examinations",  "Insurance");
    public static readonly Permission InsuranceClaimsRead    = Create(InsuranceClaimsReadCode,    "Read Claims",       "View claims and their lifecycle",         "Insurance");
    public static readonly Permission InsuranceClaimsUpdate  = Create(InsuranceClaimsUpdateCode,  "Update Claims",     "Submit, resubmit and adjudicate claims",  "Insurance");
    public static readonly Permission InsuranceClaimsSettle  = Create(InsuranceClaimsSettleCode,  "Settle Claims",     "Record insurance settlements",            "Insurance");

    public const string CashSessionsOpenCode    = "cash.sessions.open";
    public const string CashSessionsReadCode    = "cash.sessions.read";
    public const string CashSessionsCloseCode   = "cash.sessions.close";
    public const string CashEntriesAddCode      = "cash.entries.add";
    public const string CashHandoversReadCode   = "cash.handovers.read";
    public const string CashHandoversApproveCode = "cash.handovers.approve";
    public static readonly Permission CashSessionsOpen    = Create(CashSessionsOpenCode,    "Open Cash Sessions",     "Open a new cash session and record the opening float", "Cash");
    public static readonly Permission CashSessionsRead    = Create(CashSessionsReadCode,    "Read Cash Sessions",     "View cash sessions and their entries",                 "Cash");
    public static readonly Permission CashSessionsClose   = Create(CashSessionsCloseCode,   "Close Cash Sessions",    "Close a cash session and reconcile a counted handover","Cash");
    public static readonly Permission CashEntriesAdd      = Create(CashEntriesAddCode,      "Add Cash Entries",       "Record cash movements in a cash session",              "Cash");
    public static readonly Permission CashHandoversRead   = Create(CashHandoversReadCode,   "Read Cash Handovers",    "View cash handovers and reconciliation details",       "Cash");
    public static readonly Permission CashHandoversApprove = Create(CashHandoversApproveCode, "Approve Cash Handovers", "Approve completed cash handovers",                   "Cash");

    public const string NotificationTemplatesManageCode  = "notifications.templates.manage";
    public const string NotificationMessagesReadCode     = "notifications.messages.read";
    public const string NotificationMessagesSendCode     = "notifications.messages.send";
    public static readonly Permission NotificationTemplatesManage = Create(NotificationTemplatesManageCode,  "Manage Notification Templates", "Create, update, activate, deactivate and delete notification templates", "Notifications");
    public static readonly Permission NotificationMessagesRead    = Create(NotificationMessagesReadCode,     "Read Notification Messages",    "View the notification message ledger",                                   "Notifications");
    public static readonly Permission NotificationMessagesSend    = Create(NotificationMessagesSendCode,     "Send Notifications",            "Send SMS, email and push notifications",                                 "Notifications");

    private static readonly List<Permission> _all =
    [
        PatientsCreate, PatientsRead, PatientsUpdate, PatientsDelete,
        AppointmentsCreate, AppointmentsRead, AppointmentsUpdate, AppointmentsDelete, AppointmentsConfirm,
        ReportsCreate, ReportsRead, ReportsUpdate, ReportsDelete, ReportsExport,
        AnalyticsRead,
        UsersCreate, UsersRead, UsersUpdate, UsersDelete, UsersManageRoles,
        RolesCreate, RolesRead, RolesUpdate, RolesDelete, RolesManagePermissions,
        SettingsRead, SettingsUpdate,
        InventoryItemsCreate, InventoryItemsRead, InventoryItemsUpdate, InventoryItemsDelete,
        InventorySuppliersCreate, InventorySuppliersRead, InventorySuppliersUpdate, InventorySuppliersDelete,
        InventoryPurchaseOrdersCreate, InventoryPurchaseOrdersRead, InventoryPurchaseOrdersUpdate, InventoryPurchaseOrdersDelete,
        InventoryStockRead, InventoryStockIssue,
        ExaminationsTypesManage, ExaminationsCreate, ExaminationsRead, ExaminationsUpdate, ExaminationsDelete, ExaminationsPerform, ExaminationsCancel,
        StaffCreate, StaffRead, StaffUpdate, StaffDelete,
        EquipmentCreate, EquipmentRead, EquipmentUpdate, EquipmentDelete,
        ShiftsCreate, ShiftsRead, ShiftsUpdate, ShiftsDelete,
        LeaveCreate, LeaveRead, LeaveUpdate, LeaveDelete,
        ReferralDoctorsCreate, ReferralDoctorsRead, ReferralDoctorsUpdate, ReferralDoctorsDelete,
        PayrollRead, PayrollSalaryComponentsManage, PayrollSalaryManage, PayrollAllowancesManage, PayrollFeesManage, PayrollPayRunsManage, PayrollPayRunsRun,
        InsuranceCompaniesCreate, InsuranceCompaniesRead, InsuranceCompaniesUpdate, InsuranceCompaniesDelete,
        InsurancePoliciesCreate, InsurancePoliciesRead, InsurancePoliciesUpdate, InsurancePoliciesDelete,
        InsurancePreAuthorizationsCreate, InsurancePreAuthorizationsRead, InsurancePreAuthorizationsUpdate, InsurancePreAuthorizationsAttachDocument,
        InsuranceClaimsCreate, InsuranceClaimsRead, InsuranceClaimsUpdate, InsuranceClaimsSettle,
        CashSessionsOpen, CashSessionsRead, CashSessionsClose, CashEntriesAdd, CashHandoversRead, CashHandoversApprove,
        NotificationTemplatesManage, NotificationMessagesRead, NotificationMessagesSend
    ];

    public static IReadOnlyList<Permission> All => _all.AsReadOnly();

    public static Permission? GetByCode(string code) =>
        _all.FirstOrDefault(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

    public static bool IsValid(string code) =>
        _all.Any(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
}
