using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExaminationHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExaminationHistoryItems",
                schema: "Examinations");

            migrationBuilder.DropTable(
                name: "ExaminationHistories",
                schema: "Examinations");

            migrationBuilder.AddColumn<decimal>(
                name: "RadiologistFee",
                schema: "Examinations",
                table: "Examinations",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReferralFee",
                schema: "Examinations",
                table: "Examinations",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TechnicianFee",
                schema: "Examinations",
                table: "Examinations",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TypePrice",
                schema: "Examinations",
                table: "Examinations",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TypeStandardDurationMinutes",
                schema: "Examinations",
                table: "Examinations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                schema: "Examinations",
                table: "ExaminationItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RadiologistFee",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "ReferralFee",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "TechnicianFee",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "TypePrice",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "TypeStandardDurationMinutes",
                schema: "Examinations",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                schema: "Examinations",
                table: "ExaminationItems");

            migrationBuilder.CreateTable(
                name: "ExaminationHistories",
                schema: "Examinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ClinicalIndication = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EquipmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExaminationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExaminationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDiscountPercentage = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Paid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    RadiologistFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RadiologistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferralDoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferralFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Remaining = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TechnicianFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TechnicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeBodyPart = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TypeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TypeModality = table.Column<int>(type: "int", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TypePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TypeStandardDurationMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminationHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExaminationHistoryItems",
                schema: "Examinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExaminationHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsContrast = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    ItemCategory = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminationHistoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExaminationHistoryItems_ExaminationHistories_ExaminationHistoryId",
                        column: x => x.ExaminationHistoryId,
                        principalSchema: "Examinations",
                        principalTable: "ExaminationHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationHistories_CompletedAt",
                schema: "Examinations",
                table: "ExaminationHistories",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationHistories_ExaminationId",
                schema: "Examinations",
                table: "ExaminationHistories",
                column: "ExaminationId",
                unique: true,
                filter: "[ExaminationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationHistories_ExaminationTypeId",
                schema: "Examinations",
                table: "ExaminationHistories",
                column: "ExaminationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationHistoryItems_ExaminationHistoryId",
                schema: "Examinations",
                table: "ExaminationHistoryItems",
                column: "ExaminationHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationHistoryItems_ExaminationHistoryId_ItemId",
                schema: "Examinations",
                table: "ExaminationHistoryItems",
                columns: new[] { "ExaminationHistoryId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationHistoryItems_ItemId",
                schema: "Examinations",
                table: "ExaminationHistoryItems",
                column: "ItemId");
        }
    }
}
