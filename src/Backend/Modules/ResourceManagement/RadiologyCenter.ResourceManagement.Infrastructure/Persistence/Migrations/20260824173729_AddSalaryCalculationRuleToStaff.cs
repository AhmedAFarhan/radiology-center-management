using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.ResourceManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSalaryCalculationRuleToStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalaryCalculationRule",
                schema: "ResourceManagement",
                table: "Staff",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryCalculationRule",
                schema: "ResourceManagement",
                table: "Staff");
        }
    }
}
