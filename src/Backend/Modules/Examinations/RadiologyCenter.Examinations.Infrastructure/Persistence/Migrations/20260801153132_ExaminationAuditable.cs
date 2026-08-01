using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExaminationAuditable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Examinations",
                table: "Examinations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Examinations",
                table: "Examinations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Examinations",
                table: "Examinations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Examinations",
                table: "Examinations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
