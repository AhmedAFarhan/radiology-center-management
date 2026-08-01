using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVisitEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Examinations_Visits_VisitId",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropTable(
                name: "Visits",
                schema: "Examinations");

            migrationBuilder.DropColumn(
                name: "VisitId",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.RenameColumn(
                name: "VisitId",
                schema: "Examinations",
                table: "Examinations",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Examinations_VisitId",
                schema: "Examinations",
                table: "Examinations",
                newName: "IX_Examinations_PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PatientId",
                schema: "Examinations",
                table: "Examinations",
                newName: "VisitId");

            migrationBuilder.RenameIndex(
                name: "IX_Examinations_PatientId",
                schema: "Examinations",
                table: "Examinations",
                newName: "IX_Examinations_VisitId");

            migrationBuilder.AddColumn<Guid>(
                name: "VisitId",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Visits",
                schema: "Examinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    VisitedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_PatientId",
                schema: "Examinations",
                table: "Visits",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_VisitedAt",
                schema: "Examinations",
                table: "Visits",
                column: "VisitedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_Examinations_Visits_VisitId",
                schema: "Examinations",
                table: "Examinations",
                column: "VisitId",
                principalSchema: "Examinations",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
