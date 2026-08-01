using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExaminationBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                schema: "Examinations",
                table: "Examinations",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscountPercentage",
                schema: "Examinations",
                table: "Examinations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Paid",
                schema: "Examinations",
                table: "Examinations",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                schema: "Examinations",
                table: "Examinations",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Remaining",
                schema: "Examinations",
                table: "Examinations",
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
                name: "Discount",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "IsDiscountPercentage",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "Paid",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "Price",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "Remaining",
                schema: "Examinations",
                table: "Examinations");
        }
    }
}
