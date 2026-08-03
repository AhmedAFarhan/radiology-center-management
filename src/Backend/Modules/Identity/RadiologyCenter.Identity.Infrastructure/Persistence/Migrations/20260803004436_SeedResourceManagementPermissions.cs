using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedResourceManagementPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Identity",
                table: "AspNetPermissions",
                columns: new[] { "Id", "Code", "Description", "Group", "Name" },
                values: new object[,]
                {
                    { new Guid("26a1fa35-132a-d4a3-90ca-ba0fc0a78a39"), "resources.equipment.read", "View equipment", "Resources", "Read Equipment" },
                    { new Guid("402f63c9-2c76-0acc-6e19-d80c01fe35d2"), "resources.shifts.update", "Modify work shifts", "Resources", "Update Work Shifts" },
                    { new Guid("4b4bcccb-78a8-9410-5fa2-0b2b5ad02b2e"), "resources.shifts.read", "View work shifts", "Resources", "Read Work Shifts" },
                    { new Guid("4ff54837-8f20-a8b5-93aa-5c626742a079"), "resources.staff.delete", "Remove staff members", "Resources", "Delete Staff" },
                    { new Guid("7415bdf8-1d07-18a7-136e-0d08cd337e2c"), "resources.referrals.read", "View referral doctors", "Resources", "Read Referral Doctors" },
                    { new Guid("805ea172-9edb-bb9b-3fdd-fde364ecbe07"), "resources.referrals.create", "Add new referral doctors", "Resources", "Create Referral Doctors" },
                    { new Guid("80dcaed2-5408-90c5-33ed-a0bad9c12f73"), "resources.shifts.delete", "Remove work shifts", "Resources", "Delete Work Shifts" },
                    { new Guid("891ac850-50b8-c0fd-75d4-72f57935a9cf"), "resources.shifts.create", "Add new work shifts", "Resources", "Create Work Shifts" },
                    { new Guid("90420a38-f060-a86e-fb75-ca18548d1331"), "resources.leave.create", "Add new leave records", "Resources", "Create Leave" },
                    { new Guid("9cd729b1-31ce-5e23-0590-ec97a7b105d5"), "resources.leave.update", "Modify leave records", "Resources", "Update Leave" },
                    { new Guid("a15b39d5-580d-d2bc-109e-d2dcdfbfabaa"), "resources.equipment.create", "Add new equipment", "Resources", "Create Equipment" },
                    { new Guid("a9e36669-7b2a-d91f-10ca-d5899b40d9c9"), "resources.referrals.update", "Modify referral doctors", "Resources", "Update Referral Doctors" },
                    { new Guid("b621ec27-1a85-5071-31d6-c48d94fdc07e"), "resources.staff.read", "View staff members", "Resources", "Read Staff" },
                    { new Guid("c09e4f7a-1fca-53f3-36bc-a16070f6e2cd"), "resources.equipment.update", "Modify equipment and its status", "Resources", "Update Equipment" },
                    { new Guid("d34aa12a-13ac-fb27-68a7-2a8fcaf070dd"), "resources.equipment.delete", "Remove equipment", "Resources", "Delete Equipment" },
                    { new Guid("d8a19a08-1c83-5859-0163-7ce116ccbf1d"), "resources.staff.create", "Add new staff members", "Resources", "Create Staff" },
                    { new Guid("e0083532-de12-5242-d6db-6ecf4cc91719"), "resources.referrals.delete", "Remove referral doctors", "Resources", "Delete Referral Doctors" },
                    { new Guid("f3b4c0c6-ed22-c0e9-bb34-df2f3fb348a1"), "resources.leave.read", "View leave records", "Resources", "Read Leave" },
                    { new Guid("faf46329-5963-eb14-13e3-957757d84b62"), "resources.staff.update", "Modify staff members", "Resources", "Update Staff" },
                    { new Guid("fd9f1ca5-ef9b-f26a-a228-8ab82baf9b5d"), "resources.leave.delete", "Remove leave records", "Resources", "Delete Leave" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("26a1fa35-132a-d4a3-90ca-ba0fc0a78a39"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("402f63c9-2c76-0acc-6e19-d80c01fe35d2"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("4b4bcccb-78a8-9410-5fa2-0b2b5ad02b2e"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("4ff54837-8f20-a8b5-93aa-5c626742a079"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("7415bdf8-1d07-18a7-136e-0d08cd337e2c"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("805ea172-9edb-bb9b-3fdd-fde364ecbe07"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("80dcaed2-5408-90c5-33ed-a0bad9c12f73"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("891ac850-50b8-c0fd-75d4-72f57935a9cf"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("90420a38-f060-a86e-fb75-ca18548d1331"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("9cd729b1-31ce-5e23-0590-ec97a7b105d5"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("a15b39d5-580d-d2bc-109e-d2dcdfbfabaa"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("a9e36669-7b2a-d91f-10ca-d5899b40d9c9"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("b621ec27-1a85-5071-31d6-c48d94fdc07e"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("c09e4f7a-1fca-53f3-36bc-a16070f6e2cd"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("d34aa12a-13ac-fb27-68a7-2a8fcaf070dd"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("d8a19a08-1c83-5859-0163-7ce116ccbf1d"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("e0083532-de12-5242-d6db-6ecf4cc91719"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("f3b4c0c6-ed22-c0e9-bb34-df2f3fb348a1"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("faf46329-5963-eb14-13e3-957757d84b62"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("fd9f1ca5-ef9b-f26a-a228-8ab82baf9b5d"));
        }
    }
}
