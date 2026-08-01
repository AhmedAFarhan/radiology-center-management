using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExaminationHistoryBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExaminationHistories_ExaminationId",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "ExaminationId",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscountPercentage",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Paid",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Remaining",
                schema: "Examinations",
                table: "ExaminationHistories",
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
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "IsDiscountPercentage",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "Paid",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "Price",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "Remaining",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.AddColumn<Guid>(
                name: "ExaminationId",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationHistories_ExaminationId",
                schema: "Examinations",
                table: "ExaminationHistories",
                column: "ExaminationId",
                unique: true);
        }
    }
}
