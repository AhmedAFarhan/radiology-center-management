using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.CreateTable(
                name: "AspNetPermissions",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Group = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MustChangePassword = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "Identity",
                columns: table => new
                {
                    PermissionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.PermissionsId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetPermissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalSchema: "Identity",
                        principalTable: "AspNetPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "Identity",
                columns: table => new
                {
                    AssignedRolesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.AssignedRolesId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_AssignedRolesId",
                        column: x => x.AssignedRolesId,
                        principalSchema: "Identity",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                schema: "Identity",
                columns: table => new
                {
                    Token = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => new { x.UserId, x.Token });
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "AspNetPermissions",
                columns: new[] { "Id", "Code", "Description", "Group", "Name" },
                values: new object[,]
                {
                    { new Guid("00ff18fa-d4d9-c572-efd3-9c608735129c"), "appointments.delete", "Cancel or remove appointments", "Appointments", "Delete Appointments" },
                    { new Guid("0c592871-6663-c321-024b-f782e8aa258f"), "settings.read", "View system settings", "Settings", "Read Settings" },
                    { new Guid("0e0e01dc-293a-3acb-4dd7-7a8f251916d8"), "examinations.perform", "Start and complete examinations", "Examinations", "Perform Examinations" },
                    { new Guid("0f5f7960-ff37-1d18-b8db-a247701c15aa"), "inventory.items.create", "Add new inventory items", "Inventory", "Create Inventory Items" },
                    { new Guid("102e0a19-9c61-a973-203c-a2216fa0fca2"), "inventory.suppliers.create", "Add new suppliers", "Inventory", "Create Inventory Suppliers" },
                    { new Guid("121e49c3-044e-461a-62be-f488d70e32a5"), "reports.read", "View radiology reports", "Reports", "Read Reports" },
                    { new Guid("14f46857-9a94-9ca0-67ec-54a34f8b4ee7"), "examinations.create", "Create visits and add examinations", "Examinations", "Create Examinations" },
                    { new Guid("163a203a-e0e1-db23-f09c-702f14888def"), "payroll.payruns.run", "Compute, approve, reject and pay pay runs", "Payroll", "Run Pay Rolls" },
                    { new Guid("1ae22bb0-fb7f-0935-840d-f9e4061824c5"), "patients.update", "Modify existing patient records", "Patients", "Update Patients" },
                    { new Guid("1ceb83f1-b049-1e41-e030-ca764a191d37"), "patients.delete", "Remove patient records", "Patients", "Delete Patients" },
                    { new Guid("1e217b13-4ae0-c10c-1621-73cd73ed24e5"), "patients.create", "Create new patient records", "Patients", "Create Patients" },
                    { new Guid("26a1fa35-132a-d4a3-90ca-ba0fc0a78a39"), "resources.equipment.read", "View equipment", "Resources", "Read Equipment" },
                    { new Guid("2d3098d7-c9a9-5764-17d5-15b4f1ccf0a2"), "inventory.stock.issue", "Issue stock to patients", "Inventory", "Issue Stock" },
                    { new Guid("32374a33-3ce6-2306-9c99-97e5d4a55398"), "inventory.purchase-orders.update", "Modify purchase orders", "Inventory", "Update Purchase Orders" },
                    { new Guid("34aefeb7-996c-2279-4a5f-a07ee459a73d"), "users.manage-roles", "Assign or remove roles from users", "Identity", "Manage User Roles" },
                    { new Guid("39599987-0f11-0d0a-d879-7cbd0644220f"), "users.read", "View user accounts", "Identity", "Read Users" },
                    { new Guid("3bcb99ff-db6c-7983-9afd-e23a514b8d87"), "roles.update", "Modify role details", "Identity", "Update Roles" },
                    { new Guid("3df9de9b-0161-716c-2495-11ca6fe3bfe6"), "examinations.types.manage", "Create, update, delete and activate examination types", "Examinations", "Manage Examination Types" },
                    { new Guid("402f63c9-2c76-0acc-6e19-d80c01fe35d2"), "resources.shifts.update", "Modify work shifts", "Resources", "Update Work Shifts" },
                    { new Guid("41fefb26-f3c2-e784-4c76-890dee36e6eb"), "payroll.read", "View payroll records", "Payroll", "Read Payroll" },
                    { new Guid("4574d456-6962-ee5d-f344-7c87735df7a9"), "appointments.update", "Modify existing appointments", "Appointments", "Update Appointments" },
                    { new Guid("4905eb25-4c3b-fee3-c118-fb8437c960f2"), "inventory.items.delete", "Remove inventory items", "Inventory", "Delete Inventory Items" },
                    { new Guid("4933b20c-0e69-2b2a-afcc-9160c5d28e84"), "inventory.items.update", "Modify inventory items", "Inventory", "Update Inventory Items" },
                    { new Guid("4b4bcccb-78a8-9410-5fa2-0b2b5ad02b2e"), "resources.shifts.read", "View work shifts", "Resources", "Read Work Shifts" },
                    { new Guid("4cdd0e15-99f2-163d-bf02-7f9c7085639b"), "reports.export", "Export reports to external formats", "Reports", "Export Reports" },
                    { new Guid("4f1937b2-d2ab-81d3-231f-fd3f82eaeee9"), "users.delete", "Remove user accounts", "Identity", "Delete Users" },
                    { new Guid("4ff54837-8f20-a8b5-93aa-5c626742a079"), "resources.staff.delete", "Remove staff members", "Resources", "Delete Staff" },
                    { new Guid("56038185-f0f0-7c61-21cf-3d56d86c9e7b"), "appointments.create", "Schedule new appointments", "Appointments", "Create Appointments" },
                    { new Guid("5999b9cd-e387-2bb4-0fe6-ddaa9e133ee7"), "payroll.salary.manage", "Create, update, activate and delete staff salaries", "Payroll", "Manage Salaries" },
                    { new Guid("5ca0bded-ae70-5972-59c3-7541f4dae0c5"), "roles.read", "View roles", "Identity", "Read Roles" },
                    { new Guid("5e7b27cf-8d84-9117-ace6-bf62930c0d6b"), "users.create", "Create new user accounts", "Identity", "Create Users" },
                    { new Guid("65723bc4-bc7f-cfc2-e1bc-c73ccb86d023"), "patients.read", "View patient records", "Patients", "Read Patients" },
                    { new Guid("66a83501-5f9a-542f-9599-fc724b3f81e6"), "examinations.delete", "Remove examinations", "Examinations", "Delete Examinations" },
                    { new Guid("7415bdf8-1d07-18a7-136e-0d08cd337e2c"), "resources.referrals.read", "View referral doctors", "Resources", "Read Referral Doctors" },
                    { new Guid("75a5654b-9b89-dd90-0386-c2bf38eb1fa7"), "roles.create", "Create new roles", "Identity", "Create Roles" },
                    { new Guid("7fd2558b-b5b3-6988-0f52-a4330c5717f2"), "payroll.payruns.manage", "Create pay runs and manage their payslips", "Payroll", "Manage Pay Runs" },
                    { new Guid("805ea172-9edb-bb9b-3fdd-fde364ecbe07"), "resources.referrals.create", "Add new referral doctors", "Resources", "Create Referral Doctors" },
                    { new Guid("80dcaed2-5408-90c5-33ed-a0bad9c12f73"), "resources.shifts.delete", "Remove work shifts", "Resources", "Delete Work Shifts" },
                    { new Guid("88074e00-d501-1516-74f1-359b42bb70b5"), "reports.create", "Generate new radiology reports", "Reports", "Create Reports" },
                    { new Guid("891ac850-50b8-c0fd-75d4-72f57935a9cf"), "resources.shifts.create", "Add new work shifts", "Resources", "Create Work Shifts" },
                    { new Guid("8b825d03-711b-92c8-9d04-999b294a318c"), "appointments.confirm", "Confirm scheduled appointments", "Appointments", "Confirm Appointments" },
                    { new Guid("8e5a0684-bcd2-8840-b701-2312fcb7ecfa"), "roles.delete", "Remove roles", "Identity", "Delete Roles" },
                    { new Guid("90420a38-f060-a86e-fb75-ca18548d1331"), "resources.leave.create", "Add new leave records", "Resources", "Create Leave" },
                    { new Guid("9357da48-43eb-8b5a-08e1-2970f32cd4e4"), "users.update", "Modify user accounts", "Identity", "Update Users" },
                    { new Guid("97f0ba16-636a-f3d1-38b5-aa196fda0fe6"), "appointments.read", "View appointment details", "Appointments", "Read Appointments" },
                    { new Guid("98f262b3-ca68-3bba-a914-f98d3e4947e5"), "inventory.purchase-orders.delete", "Remove purchase orders", "Inventory", "Delete Purchase Orders" },
                    { new Guid("9a611184-7476-b59a-382c-5918515fc4bd"), "payroll.fees.manage", "Create, update, activate and delete examination and referral fees", "Payroll", "Manage Payroll Fees" },
                    { new Guid("9cd729b1-31ce-5e23-0590-ec97a7b105d5"), "resources.leave.update", "Modify leave records", "Resources", "Update Leave" },
                    { new Guid("a15b39d5-580d-d2bc-109e-d2dcdfbfabaa"), "resources.equipment.create", "Add new equipment", "Resources", "Create Equipment" },
                    { new Guid("a9b6b892-9cbf-5e0d-524e-342037a9c0db"), "reports.delete", "Remove reports", "Reports", "Delete Reports" },
                    { new Guid("a9e36669-7b2a-d91f-10ca-d5899b40d9c9"), "resources.referrals.update", "Modify referral doctors", "Resources", "Update Referral Doctors" },
                    { new Guid("ab653fcc-50eb-0af8-5aaf-16e746bb5385"), "inventory.suppliers.update", "Modify suppliers", "Inventory", "Update Inventory Suppliers" },
                    { new Guid("af121ffb-79e2-804a-901f-97c3c40b0c9a"), "payroll.salary-components.manage", "Create, update, activate and delete salary components", "Payroll", "Manage Salary Components" },
                    { new Guid("afb3c764-ed7b-76fb-d661-bb52f1632f24"), "reports.update", "Modify existing reports", "Reports", "Update Reports" },
                    { new Guid("b079ac2e-ee45-24cf-fc5d-26ecff766e16"), "inventory.suppliers.read", "View suppliers", "Inventory", "Read Inventory Suppliers" },
                    { new Guid("b370e3e6-f655-f445-ee3c-e3fbd74e489a"), "inventory.suppliers.delete", "Remove suppliers", "Inventory", "Delete Inventory Suppliers" },
                    { new Guid("b621ec27-1a85-5071-31d6-c48d94fdc07e"), "resources.staff.read", "View staff members", "Resources", "Read Staff" },
                    { new Guid("c09e4f7a-1fca-53f3-36bc-a16070f6e2cd"), "resources.equipment.update", "Modify equipment and its status", "Resources", "Update Equipment" },
                    { new Guid("c5ad4ec4-37cb-8d65-1f99-893d6fa9171b"), "examinations.read", "View visits and examinations", "Examinations", "Read Examinations" },
                    { new Guid("c767d3a2-dd4b-0c1b-00b0-d99ea2bfbbc8"), "examinations.update", "Modify examinations and their items", "Examinations", "Update Examinations" },
                    { new Guid("d34aa12a-13ac-fb27-68a7-2a8fcaf070dd"), "resources.equipment.delete", "Remove equipment", "Resources", "Delete Equipment" },
                    { new Guid("d8a19a08-1c83-5859-0163-7ce116ccbf1d"), "resources.staff.create", "Add new staff members", "Resources", "Create Staff" },
                    { new Guid("dd84e7dd-c45f-4441-6c18-4deb65b3830c"), "examinations.cancel", "Cancel visits and examinations", "Examinations", "Cancel Examinations" },
                    { new Guid("df6c1ff4-ee40-8ae2-df55-166358e7bdcb"), "inventory.purchase-orders.create", "Create purchase orders", "Inventory", "Create Purchase Orders" },
                    { new Guid("e0083532-de12-5242-d6db-6ecf4cc91719"), "resources.referrals.delete", "Remove referral doctors", "Resources", "Delete Referral Doctors" },
                    { new Guid("e0548f02-0b69-0aa7-d0c3-6b4f5b7a69dd"), "settings.update", "Modify system settings", "Settings", "Update Settings" },
                    { new Guid("e66a4a79-01e6-8e89-c8c5-d0ce5044fa01"), "roles.manage-permissions", "Assign or remove permissions from roles", "Identity", "Manage Role Permissions" },
                    { new Guid("e6f1e4d7-92ab-2021-daab-ee4841a4e5e2"), "inventory.stock.read", "View stock levels and movements", "Inventory", "Read Stock" },
                    { new Guid("ecb71584-005e-bc07-3cea-0e99b44ce6b5"), "payroll.allowances.manage", "Create, update, activate and delete allowance assignments", "Payroll", "Manage Allowances" },
                    { new Guid("f3b4c0c6-ed22-c0e9-bb34-df2f3fb348a1"), "resources.leave.read", "View leave records", "Resources", "Read Leave" },
                    { new Guid("faac6b2a-dca3-cef4-efd0-20060a4de1e4"), "inventory.items.read", "View inventory items", "Inventory", "Read Inventory Items" },
                    { new Guid("faf46329-5963-eb14-13e3-957757d84b62"), "resources.staff.update", "Modify staff members", "Resources", "Update Staff" },
                    { new Guid("fd9f1ca5-ef9b-f26a-a228-8ab82baf9b5d"), "resources.leave.delete", "Remove leave records", "Resources", "Delete Leave" },
                    { new Guid("fe88b74c-0e4d-4306-e2c1-961f38291ea0"), "inventory.purchase-orders.read", "View purchase orders", "Inventory", "Read Purchase Orders" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPermissions_Code",
                schema: "Identity",
                table: "AspNetPermissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                schema: "Identity",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                schema: "Identity",
                table: "RolePermissions",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "RefreshToken",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "AspNetPermissions",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "Identity");
        }
    }
}
