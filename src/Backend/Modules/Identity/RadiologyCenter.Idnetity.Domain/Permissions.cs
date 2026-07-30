using RadiologyCenter.Idnetity.Domain.Entities;

namespace RadiologyCenter.Idnetity.Domain;

public static class Permissions
{
    private static Permission Create(string code, string name, string? description = null, string? group = null) =>
        new(Permission.CreateDeterministicId(code), code, name, description, group);

    public static readonly Permission PatientsCreate  = Create("patients.create",  "Create Patients",  "Create new patient records",     "Patients");
    public static readonly Permission PatientsRead    = Create("patients.read",    "Read Patients",    "View patient records",           "Patients");
    public static readonly Permission PatientsUpdate  = Create("patients.update",  "Update Patients",  "Modify existing patient records", "Patients");
    public static readonly Permission PatientsDelete  = Create("patients.delete",  "Delete Patients",  "Remove patient records",          "Patients");

    public static readonly Permission AppointmentsCreate  = Create("appointments.create",  "Create Appointments",  "Schedule new appointments",            "Appointments");
    public static readonly Permission AppointmentsRead    = Create("appointments.read",    "Read Appointments",    "View appointment details",             "Appointments");
    public static readonly Permission AppointmentsUpdate  = Create("appointments.update",  "Update Appointments",  "Modify existing appointments",         "Appointments");
    public static readonly Permission AppointmentsDelete  = Create("appointments.delete",  "Delete Appointments",  "Cancel or remove appointments",        "Appointments");
    public static readonly Permission AppointmentsConfirm = Create("appointments.confirm", "Confirm Appointments", "Confirm scheduled appointments",       "Appointments");

    public static readonly Permission ReportsCreate  = Create("reports.create",  "Create Reports",  "Generate new radiology reports",  "Reports");
    public static readonly Permission ReportsRead    = Create("reports.read",    "Read Reports",    "View radiology reports",          "Reports");
    public static readonly Permission ReportsUpdate  = Create("reports.update",  "Update Reports",  "Modify existing reports",         "Reports");
    public static readonly Permission ReportsDelete  = Create("reports.delete",  "Delete Reports",  "Remove reports",                  "Reports");
    public static readonly Permission ReportsExport  = Create("reports.export",  "Export Reports",  "Export reports to external formats", "Reports");

    public static readonly Permission UsersCreate      = Create("users.create",       "Create Users",        "Create new user accounts",              "Identity");
    public static readonly Permission UsersRead        = Create("users.read",         "Read Users",          "View user accounts",                    "Identity");
    public static readonly Permission UsersUpdate      = Create("users.update",       "Update Users",        "Modify user accounts",                  "Identity");
    public static readonly Permission UsersDelete      = Create("users.delete",       "Delete Users",        "Remove user accounts",                  "Identity");
    public static readonly Permission UsersManageRoles = Create("users.manage-roles", "Manage User Roles",   "Assign or remove roles from users",     "Identity");

    public static readonly Permission RolesCreate          = Create("roles.create",           "Create Roles",            "Create new roles",                        "Identity");
    public static readonly Permission RolesRead            = Create("roles.read",             "Read Roles",              "View roles",                              "Identity");
    public static readonly Permission RolesUpdate          = Create("roles.update",           "Update Roles",            "Modify role details",                     "Identity");
    public static readonly Permission RolesDelete          = Create("roles.delete",           "Delete Roles",            "Remove roles",                            "Identity");
    public static readonly Permission RolesManagePermissions = Create("roles.manage-permissions", "Manage Role Permissions", "Assign or remove permissions from roles", "Identity");

    public static readonly Permission SettingsRead   = Create("settings.read",   "Read Settings",   "View system settings",   "Settings");
    public static readonly Permission SettingsUpdate = Create("settings.update", "Update Settings", "Modify system settings", "Settings");

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
