using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExaminationHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExaminationHistories",
                schema: "Examinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExaminationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExaminationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TypeModality = table.Column<int>(type: "int", nullable: false),
                    TypeBodyPart = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TypePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TypeStandardDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    ReferringDoctor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClinicalIndication = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
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
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ItemCategory = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsContrast = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
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
                unique: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExaminationHistoryItems",
                schema: "Examinations");

            migrationBuilder.DropTable(
                name: "ExaminationHistories",
                schema: "Examinations");
        }
    }
}
