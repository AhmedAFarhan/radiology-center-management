using RadiologyCenter.Idnetity.Domain.Entities;

namespace RadiologyCenter.Idnetity.Domain;

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

    private static readonly List<Permission> _all =
    [
        PatientsCreate, PatientsRead, PatientsUpdate, PatientsDelete,
        AppointmentsCreate, AppointmentsRead, AppointmentsUpdate, AppointmentsDelete, AppointmentsConfirm,
        ReportsCreate, ReportsRead, ReportsUpdate, ReportsDelete, ReportsExport,
        UsersCreate, UsersRead, UsersUpdate, UsersDelete, UsersManageRoles,
        RolesCreate, RolesRead, RolesUpdate, RolesDelete, RolesManagePermissions,
        SettingsRead, SettingsUpdate
    ];

    public static IReadOnlyList<Permission> All => _all.AsReadOnly();

    public static Permission? GetByCode(string code) =>
        _all.FirstOrDefault(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

    public static bool IsValid(string code) =>
        _all.Any(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
}
