using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExaminationTypeCodeUniqueAmongActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExaminationTypes_Code",
                schema: "Examinations",
                table: "ExaminationTypes");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationTypes_Code",
                schema: "Examinations",
                table: "ExaminationTypes",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExaminationTypes_Code",
                schema: "Examinations",
                table: "ExaminationTypes");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationTypes_Code",
                schema: "Examinations",
                table: "ExaminationTypes",
                column: "Code",
                unique: true);
        }
    }
}
