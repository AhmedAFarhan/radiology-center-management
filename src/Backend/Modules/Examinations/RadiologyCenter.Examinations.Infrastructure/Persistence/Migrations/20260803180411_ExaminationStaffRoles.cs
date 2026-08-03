using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExaminationStaffRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferringDoctor",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "ReferringDoctor",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.AddColumn<Guid>(
                name: "RadiologistId",
                schema: "Examinations",
                table: "Examinations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReferralDoctorId",
                schema: "Examinations",
                table: "Examinations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TechnicianId",
                schema: "Examinations",
                table: "Examinations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RadiologistId",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReferralDoctorId",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TechnicianId",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RadiologistId",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "ReferralDoctorId",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "RadiologistId",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "ReferralDoctorId",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                schema: "Examinations",
                table: "ExaminationHistories");

            migrationBuilder.AddColumn<string>(
                name: "ReferringDoctor",
                schema: "Examinations",
                table: "Examinations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferringDoctor",
                schema: "Examinations",
                table: "ExaminationHistories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
