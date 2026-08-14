using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExaminationIdToHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExaminationId",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationHistories_ExaminationId",
                schema: "Examinations",
                table: "ExaminationHistories",
                column: "ExaminationId",
                unique: true,
                filter: "[ExaminationId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExaminationHistories_ExaminationId",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "ExaminationId",
                schema: "Examinations",
                table: "ExaminationHistories");
        }
    }
}
