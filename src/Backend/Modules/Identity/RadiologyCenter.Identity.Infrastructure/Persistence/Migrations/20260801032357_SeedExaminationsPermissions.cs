using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedExaminationsPermissions : Migration
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
                    { new Guid("0e0e01dc-293a-3acb-4dd7-7a8f251916d8"), "examinations.perform", "Start and complete examinations", "Examinations", "Perform Examinations" },
                    { new Guid("14f46857-9a94-9ca0-67ec-54a34f8b4ee7"), "examinations.create", "Create visits and add examinations", "Examinations", "Create Examinations" },
                    { new Guid("3df9de9b-0161-716c-2495-11ca6fe3bfe6"), "examinations.types.manage", "Create, update, delete and activate examination types", "Examinations", "Manage Examination Types" },
                    { new Guid("66a83501-5f9a-542f-9599-fc724b3f81e6"), "examinations.delete", "Remove examinations", "Examinations", "Delete Examinations" },
                    { new Guid("c5ad4ec4-37cb-8d65-1f99-893d6fa9171b"), "examinations.read", "View visits and examinations", "Examinations", "Read Examinations" },
                    { new Guid("c767d3a2-dd4b-0c1b-00b0-d99ea2bfbbc8"), "examinations.update", "Modify examinations and their items", "Examinations", "Update Examinations" },
                    { new Guid("dd84e7dd-c45f-4441-6c18-4deb65b3830c"), "examinations.cancel", "Cancel visits and examinations", "Examinations", "Cancel Examinations" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("0e0e01dc-293a-3acb-4dd7-7a8f251916d8"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("14f46857-9a94-9ca0-67ec-54a34f8b4ee7"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("3df9de9b-0161-716c-2495-11ca6fe3bfe6"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("66a83501-5f9a-542f-9599-fc724b3f81e6"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("c5ad4ec4-37cb-8d65-1f99-893d6fa9171b"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("c767d3a2-dd4b-0c1b-00b0-d99ea2bfbbc8"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("dd84e7dd-c45f-4441-6c18-4deb65b3830c"));
        }
    }
}
