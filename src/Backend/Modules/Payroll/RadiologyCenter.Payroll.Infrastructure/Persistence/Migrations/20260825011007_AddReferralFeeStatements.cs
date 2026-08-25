using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Payroll.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReferralFeeStatements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReferralFeeStatements",
                schema: "Payroll",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferralDoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExamCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralFeeStatements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferralFeeStatements_PayRuns_PayRunId",
                        column: x => x.PayRunId,
                        principalSchema: "Payroll",
                        principalTable: "PayRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReferralFeeStatements_PayRunId_ReferralDoctorId",
                schema: "Payroll",
                table: "ReferralFeeStatements",
                columns: new[] { "PayRunId", "ReferralDoctorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReferralFeeStatements",
                schema: "Payroll");
        }
    }
}
