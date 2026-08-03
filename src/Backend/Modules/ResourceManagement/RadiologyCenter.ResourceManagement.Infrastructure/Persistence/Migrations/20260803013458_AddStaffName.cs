using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.ResourceManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "ResourceManagement",
                table: "Staff",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "ResourceManagement",
                table: "Staff",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                schema: "ResourceManagement",
                table: "Staff",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "ResourceManagement",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "ResourceManagement",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                schema: "ResourceManagement",
                table: "Staff");
        }
    }
}
