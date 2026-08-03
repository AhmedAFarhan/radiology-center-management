using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.ResourceManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReferralDoctorNameSplitAndUniquePhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "ResourceManagement",
                table: "ReferralDoctors");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "ResourceManagement",
                table: "ReferralDoctors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "ResourceManagement",
                table: "ReferralDoctors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                schema: "ResourceManagement",
                table: "ReferralDoctors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferralDoctors_Phone",
                schema: "ResourceManagement",
                table: "ReferralDoctors",
                column: "Phone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReferralDoctors_Phone",
                schema: "ResourceManagement",
                table: "ReferralDoctors");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "ResourceManagement",
                table: "ReferralDoctors");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "ResourceManagement",
                table: "ReferralDoctors");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                schema: "ResourceManagement",
                table: "ReferralDoctors");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "ResourceManagement",
                table: "ReferralDoctors",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
