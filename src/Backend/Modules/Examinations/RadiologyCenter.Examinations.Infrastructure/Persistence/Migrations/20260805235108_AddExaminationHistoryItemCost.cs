using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExaminationHistoryItemCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                schema: "Examinations",
                table: "ExaminationHistoryItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitCost",
                schema: "Examinations",
                table: "ExaminationHistoryItems");
        }
    }
}
