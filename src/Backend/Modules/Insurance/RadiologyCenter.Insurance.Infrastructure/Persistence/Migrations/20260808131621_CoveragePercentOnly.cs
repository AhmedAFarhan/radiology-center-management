using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Insurance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CoveragePercentOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Copay",
                schema: "Insurance",
                table: "InsurancePolicies");

            migrationBuilder.DropColumn(
                name: "Deductible",
                schema: "Insurance",
                table: "InsurancePolicies");

            migrationBuilder.DropColumn(
                name: "CopayApplied",
                schema: "Insurance",
                table: "Claims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Copay",
                schema: "Insurance",
                table: "InsurancePolicies",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Deductible",
                schema: "Insurance",
                table: "InsurancePolicies",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CopayApplied",
                schema: "Insurance",
                table: "Claims",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
