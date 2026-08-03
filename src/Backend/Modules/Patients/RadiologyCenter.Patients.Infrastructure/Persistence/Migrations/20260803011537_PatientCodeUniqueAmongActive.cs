using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Patients.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PatientCodeUniqueAmongActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_PatientCode",
                schema: "Patients",
                table: "Patients");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PatientCode",
                schema: "Patients",
                table: "Patients",
                column: "PatientCode",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_PatientCode",
                schema: "Patients",
                table: "Patients");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PatientCode",
                schema: "Patients",
                table: "Patients",
                column: "PatientCode",
                unique: true);
        }
    }
}
