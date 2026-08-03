using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.ResourceManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStaffEmployeeNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Staff_EmployeeNumber",
                schema: "ResourceManagement",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "EmployeeNumber",
                schema: "ResourceManagement",
                table: "Staff");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeNumber",
                schema: "ResourceManagement",
                table: "Staff",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_EmployeeNumber",
                schema: "ResourceManagement",
                table: "Staff",
                column: "EmployeeNumber",
                unique: true);
        }
    }
}
