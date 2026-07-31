using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Patients.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PatientSequenceYearNonIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientNumberSequences",
                schema: "Patients",
                table: "PatientNumberSequences");

            migrationBuilder.DropColumn(
                name: "Year",
                schema: "Patients",
                table: "PatientNumberSequences");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                schema: "Patients",
                table: "PatientNumberSequences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientNumberSequences",
                schema: "Patients",
                table: "PatientNumberSequences",
                column: "Year");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientNumberSequences",
                schema: "Patients",
                table: "PatientNumberSequences");

            migrationBuilder.DropColumn(
                name: "Year",
                schema: "Patients",
                table: "PatientNumberSequences");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                schema: "Patients",
                table: "PatientNumberSequences",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientNumberSequences",
                schema: "Patients",
                table: "PatientNumberSequences",
                column: "Year");
        }
    }
}
