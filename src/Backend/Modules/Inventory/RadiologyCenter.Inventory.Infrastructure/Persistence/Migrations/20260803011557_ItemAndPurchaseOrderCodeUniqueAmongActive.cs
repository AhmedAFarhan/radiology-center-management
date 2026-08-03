using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Inventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ItemAndPurchaseOrderCodeUniqueAmongActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_OrderNumber",
                schema: "Inventory",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_Items_Name",
                schema: "Inventory",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_OrderNumber",
                schema: "Inventory",
                table: "PurchaseOrders",
                column: "OrderNumber",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Name",
                schema: "Inventory",
                table: "Items",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_OrderNumber",
                schema: "Inventory",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_Items_Name",
                schema: "Inventory",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_OrderNumber",
                schema: "Inventory",
                table: "PurchaseOrders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_Name",
                schema: "Inventory",
                table: "Items",
                column: "Name",
                unique: true);
        }
    }
}
