using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Patients.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePatientNumberSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [System].[NumberSequences] ([Name], [Year], [LastNumber])
                SELECT 'Patient', [Year], [LastNumber] FROM [Patients].[PatientNumberSequences];
                """);

            migrationBuilder.DropTable(
                name: "PatientNumberSequences",
                schema: "Patients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientNumberSequences",
                schema: "Patients",
                columns: table => new
                {
                    Year = table.Column<int>(type: "int", nullable: false),
                    LastNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientNumberSequences", x => x.Year);
                });
        }
    }
}
