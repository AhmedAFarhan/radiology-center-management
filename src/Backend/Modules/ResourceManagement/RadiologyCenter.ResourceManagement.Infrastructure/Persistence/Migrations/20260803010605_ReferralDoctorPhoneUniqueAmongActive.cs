using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.ResourceManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReferralDoctorPhoneUniqueAmongActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReferralDoctors_Phone",
                schema: "ResourceManagement",
                table: "ReferralDoctors");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralDoctors_Phone",
                schema: "ResourceManagement",
                table: "ReferralDoctors",
                column: "Phone",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReferralDoctors_Phone",
                schema: "ResourceManagement",
                table: "ReferralDoctors");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralDoctors_Phone",
                schema: "ResourceManagement",
                table: "ReferralDoctors",
                column: "Phone",
                unique: true);
        }
    }
}
