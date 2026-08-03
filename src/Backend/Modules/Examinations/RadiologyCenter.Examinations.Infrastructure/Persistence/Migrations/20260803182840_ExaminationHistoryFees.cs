using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExaminationHistoryFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RadiologistFee",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReferralFee",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TechnicianFee",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RadiologistFee",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "ReferralFee",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "TechnicianFee",
                schema: "Examinations",
                table: "ExaminationHistories");
        }
    }
}
