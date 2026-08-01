using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedInventoryPermissions : Migration
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
                    { new Guid("0f5f7960-ff37-1d18-b8db-a247701c15aa"), "inventory.items.create", "Add new inventory items", "Inventory", "Create Inventory Items" },
                    { new Guid("102e0a19-9c61-a973-203c-a2216fa0fca2"), "inventory.suppliers.create", "Add new suppliers", "Inventory", "Create Inventory Suppliers" },
                    { new Guid("2d3098d7-c9a9-5764-17d5-15b4f1ccf0a2"), "inventory.stock.issue", "Issue stock to patients", "Inventory", "Issue Stock" },
                    { new Guid("32374a33-3ce6-2306-9c99-97e5d4a55398"), "inventory.purchase-orders.update", "Modify purchase orders", "Inventory", "Update Purchase Orders" },
                    { new Guid("4905eb25-4c3b-fee3-c118-fb8437c960f2"), "inventory.items.delete", "Remove inventory items", "Inventory", "Delete Inventory Items" },
                    { new Guid("4933b20c-0e69-2b2a-afcc-9160c5d28e84"), "inventory.items.update", "Modify inventory items", "Inventory", "Update Inventory Items" },
                    { new Guid("98f262b3-ca68-3bba-a914-f98d3e4947e5"), "inventory.purchase-orders.delete", "Remove purchase orders", "Inventory", "Delete Purchase Orders" },
                    { new Guid("ab653fcc-50eb-0af8-5aaf-16e746bb5385"), "inventory.suppliers.update", "Modify suppliers", "Inventory", "Update Inventory Suppliers" },
                    { new Guid("b079ac2e-ee45-24cf-fc5d-26ecff766e16"), "inventory.suppliers.read", "View suppliers", "Inventory", "Read Inventory Suppliers" },
                    { new Guid("b370e3e6-f655-f445-ee3c-e3fbd74e489a"), "inventory.suppliers.delete", "Remove suppliers", "Inventory", "Delete Inventory Suppliers" },
                    { new Guid("df6c1ff4-ee40-8ae2-df55-166358e7bdcb"), "inventory.purchase-orders.create", "Create purchase orders", "Inventory", "Create Purchase Orders" },
                    { new Guid("e6f1e4d7-92ab-2021-daab-ee4841a4e5e2"), "inventory.stock.read", "View stock levels and movements", "Inventory", "Read Stock" },
                    { new Guid("faac6b2a-dca3-cef4-efd0-20060a4de1e4"), "inventory.items.read", "View inventory items", "Inventory", "Read Inventory Items" },
                    { new Guid("fe88b74c-0e4d-4306-e2c1-961f38291ea0"), "inventory.purchase-orders.read", "View purchase orders", "Inventory", "Read Purchase Orders" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("0f5f7960-ff37-1d18-b8db-a247701c15aa"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("102e0a19-9c61-a973-203c-a2216fa0fca2"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("2d3098d7-c9a9-5764-17d5-15b4f1ccf0a2"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("32374a33-3ce6-2306-9c99-97e5d4a55398"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("4905eb25-4c3b-fee3-c118-fb8437c960f2"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("4933b20c-0e69-2b2a-afcc-9160c5d28e84"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("98f262b3-ca68-3bba-a914-f98d3e4947e5"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("ab653fcc-50eb-0af8-5aaf-16e746bb5385"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("b079ac2e-ee45-24cf-fc5d-26ecff766e16"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("b370e3e6-f655-f445-ee3c-e3fbd74e489a"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("df6c1ff4-ee40-8ae2-df55-166358e7bdcb"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("e6f1e4d7-92ab-2021-daab-ee4841a4e5e2"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("faac6b2a-dca3-cef4-efd0-20060a4de1e4"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("fe88b74c-0e4d-4306-e2c1-961f38291ea0"));
        }
    }
}
