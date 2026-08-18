using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExaminationPacsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessionNumber",
                schema: "Examinations",
                table: "Examinations",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ImagesReceivedAt",
                schema: "Examinations",
                table: "Examinations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudyInstanceUID",
                schema: "Examinations",
                table: "Examinations",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessionNumber",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "ImagesReceivedAt",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "StudyInstanceUID",
                schema: "Examinations",
                table: "Examinations");
        }
    }
}
