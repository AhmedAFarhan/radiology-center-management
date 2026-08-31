using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeZoneId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                schema: "Identity",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Africa/Cairo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                schema: "Identity",
                table: "AspNetUsers");
        }
    }
}
