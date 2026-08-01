using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Inventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrderNumberSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [System].[NumberSequences] ([Name], [Year], [LastNumber])
                SELECT 'PurchaseOrder', [Year], [LastNumber] FROM [Inventory].[OrderNumberSequences];
                """);

            migrationBuilder.DropTable(
                name: "OrderNumberSequences",
                schema: "Inventory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderNumberSequences",
                schema: "Inventory",
                columns: table => new
                {
                    Year = table.Column<int>(type: "int", nullable: false),
                    LastNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderNumberSequences", x => x.Year);
                });
        }
    }
}
