using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Inventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStockBatchConcurrencyAndCheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "Inventory",
                table: "StockBatches",
                type: "rowversion",
                rowVersion: true,
                nullable: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_StockBatches_QuantityRemaining_NonNegative",
                schema: "Inventory",
                table: "StockBatches",
                sql: "[QuantityRemaining] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_StockBatches_QuantityRemaining_NonNegative",
                schema: "Inventory",
                table: "StockBatches");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "Inventory",
                table: "StockBatches");
        }
    }
}
