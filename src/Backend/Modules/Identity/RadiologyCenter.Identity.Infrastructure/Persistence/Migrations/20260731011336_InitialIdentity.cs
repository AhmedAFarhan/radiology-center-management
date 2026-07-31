using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentity : Migration
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
                    { new Guid("121e49c3-044e-461a-62be-f488d70e32a5"), "reports.read", "View radiology reports", "Reports", "Read Reports" },
                    { new Guid("1ae22bb0-fb7f-0935-840d-f9e4061824c5"), "patients.update", "Modify existing patient records", "Patients", "Update Patients" },
                    { new Guid("1ceb83f1-b049-1e41-e030-ca764a191d37"), "patients.delete", "Remove patient records", "Patients", "Delete Patients" },
                    { new Guid("1e217b13-4ae0-c10c-1621-73cd73ed24e5"), "patients.create", "Create new patient records", "Patients", "Create Patients" },
                    { new Guid("34aefeb7-996c-2279-4a5f-a07ee459a73d"), "users.manage-roles", "Assign or remove roles from users", "Identity", "Manage User Roles" },
                    { new Guid("39599987-0f11-0d0a-d879-7cbd0644220f"), "users.read", "View user accounts", "Identity", "Read Users" },
                    { new Guid("3bcb99ff-db6c-7983-9afd-e23a514b8d87"), "roles.update", "Modify role details", "Identity", "Update Roles" },
                    { new Guid("4574d456-6962-ee5d-f344-7c87735df7a9"), "appointments.update", "Modify existing appointments", "Appointments", "Update Appointments" },
                    { new Guid("4cdd0e15-99f2-163d-bf02-7f9c7085639b"), "reports.export", "Export reports to external formats", "Reports", "Export Reports" },
                    { new Guid("4f1937b2-d2ab-81d3-231f-fd3f82eaeee9"), "users.delete", "Remove user accounts", "Identity", "Delete Users" },
                    { new Guid("56038185-f0f0-7c61-21cf-3d56d86c9e7b"), "appointments.create", "Schedule new appointments", "Appointments", "Create Appointments" },
                    { new Guid("5ca0bded-ae70-5972-59c3-7541f4dae0c5"), "roles.read", "View roles", "Identity", "Read Roles" },
                    { new Guid("5e7b27cf-8d84-9117-ace6-bf62930c0d6b"), "users.create", "Create new user accounts", "Identity", "Create Users" },
                    { new Guid("65723bc4-bc7f-cfc2-e1bc-c73ccb86d023"), "patients.read", "View patient records", "Patients", "Read Patients" },
                    { new Guid("75a5654b-9b89-dd90-0386-c2bf38eb1fa7"), "roles.create", "Create new roles", "Identity", "Create Roles" },
                    { new Guid("88074e00-d501-1516-74f1-359b42bb70b5"), "reports.create", "Generate new radiology reports", "Reports", "Create Reports" },
                    { new Guid("8b825d03-711b-92c8-9d04-999b294a318c"), "appointments.confirm", "Confirm scheduled appointments", "Appointments", "Confirm Appointments" },
                    { new Guid("8e5a0684-bcd2-8840-b701-2312fcb7ecfa"), "roles.delete", "Remove roles", "Identity", "Delete Roles" },
                    { new Guid("9357da48-43eb-8b5a-08e1-2970f32cd4e4"), "users.update", "Modify user accounts", "Identity", "Update Users" },
                    { new Guid("97f0ba16-636a-f3d1-38b5-aa196fda0fe6"), "appointments.read", "View appointment details", "Appointments", "Read Appointments" },
                    { new Guid("a9b6b892-9cbf-5e0d-524e-342037a9c0db"), "reports.delete", "Remove reports", "Reports", "Delete Reports" },
                    { new Guid("afb3c764-ed7b-76fb-d661-bb52f1632f24"), "reports.update", "Modify existing reports", "Reports", "Update Reports" },
                    { new Guid("e0548f02-0b69-0aa7-d0c3-6b4f5b7a69dd"), "settings.update", "Modify system settings", "Settings", "Update Settings" },
                    { new Guid("e66a4a79-01e6-8e89-c8c5-d0ce5044fa01"), "roles.manage-permissions", "Assign or remove permissions from roles", "Identity", "Manage Role Permissions" }
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
