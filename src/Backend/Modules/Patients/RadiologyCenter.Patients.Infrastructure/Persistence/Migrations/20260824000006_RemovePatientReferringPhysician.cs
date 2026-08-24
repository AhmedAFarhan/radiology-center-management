using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Patients.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePatientReferringPhysician : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferringPhysician",
                schema: "Patients",
                table: "Patients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferringPhysician",
                schema: "Patients",
                table: "Patients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
