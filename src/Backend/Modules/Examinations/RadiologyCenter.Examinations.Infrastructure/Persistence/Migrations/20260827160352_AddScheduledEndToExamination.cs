using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduledEndToExamination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledEnd",
                schema: "Examinations",
                table: "Examinations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_ScheduledAt_ScheduledEnd",
                schema: "Examinations",
                table: "Examinations",
                columns: new[] { "ScheduledAt", "ScheduledEnd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Examinations_ScheduledAt_ScheduledEnd",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "ScheduledEnd",
                schema: "Examinations",
                table: "Examinations");
        }
    }
}
