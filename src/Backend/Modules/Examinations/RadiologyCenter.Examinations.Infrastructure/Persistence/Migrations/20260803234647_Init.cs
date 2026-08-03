using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Examinations");

            migrationBuilder.CreateTable(
                name: "ExaminationHistories",
                schema: "Examinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExaminationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TypeModality = table.Column<int>(type: "int", nullable: false),
                    TypeBodyPart = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TypePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TypeStandardDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    ReferralDoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RadiologistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RadiologistFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TechnicianFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ReferralFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ClinicalIndication = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDiscountPercentage = table.Column<bool>(type: "bit", nullable: false),
                    Paid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remaining = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
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
                name: "Examinations",
                schema: "Examinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExaminationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferralDoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RadiologistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClinicalIndication = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDiscountPercentage = table.Column<bool>(type: "bit", nullable: false),
                    Paid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remaining = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Examinations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExaminationTypes",
                schema: "Examinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Modality = table.Column<int>(type: "int", nullable: false),
                    BodyPart = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StandardDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RequiresPreparation = table.Column<bool>(type: "bit", nullable: false),
                    RequiresConsent = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminationTypes", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "ExaminationItems",
                schema: "Examinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExaminationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsContrast = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExaminationItems_Examinations_ExaminationId",
                        column: x => x.ExaminationId,
                        principalSchema: "Examinations",
                        principalTable: "Examinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExaminationTypeItems",
                schema: "Examinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExaminationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsContrast = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminationTypeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExaminationTypeItems_ExaminationTypes_ExaminationTypeId",
                        column: x => x.ExaminationTypeId,
                        principalSchema: "Examinations",
                        principalTable: "ExaminationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationHistories_CompletedAt",
                schema: "Examinations",
                table: "ExaminationHistories",
                column: "CompletedAt");

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

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationItems_ExaminationId",
                schema: "Examinations",
                table: "ExaminationItems",
                column: "ExaminationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationItems_ItemId",
                schema: "Examinations",
                table: "ExaminationItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_ExaminationTypeId",
                schema: "Examinations",
                table: "Examinations",
                column: "ExaminationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_PatientId",
                schema: "Examinations",
                table: "Examinations",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_Status",
                schema: "Examinations",
                table: "Examinations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationTypeItems_ExaminationTypeId",
                schema: "Examinations",
                table: "ExaminationTypeItems",
                column: "ExaminationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationTypeItems_ItemId",
                schema: "Examinations",
                table: "ExaminationTypeItems",
                column: "ItemId");

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
            migrationBuilder.DropTable(
                name: "ExaminationHistoryItems",
                schema: "Examinations");

            migrationBuilder.DropTable(
                name: "ExaminationItems",
                schema: "Examinations");

            migrationBuilder.DropTable(
                name: "ExaminationTypeItems",
                schema: "Examinations");

            migrationBuilder.DropTable(
                name: "ExaminationHistories",
                schema: "Examinations");

            migrationBuilder.DropTable(
                name: "Examinations",
                schema: "Examinations");

            migrationBuilder.DropTable(
                name: "ExaminationTypes",
                schema: "Examinations");
        }
    }
}
