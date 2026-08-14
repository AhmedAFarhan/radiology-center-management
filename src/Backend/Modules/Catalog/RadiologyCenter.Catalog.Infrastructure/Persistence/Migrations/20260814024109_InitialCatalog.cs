using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Catalog");

            migrationBuilder.CreateTable(
                name: "CatalogExaminationTypes",
                schema: "Catalog",
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
                    table.PrimaryKey("PK_CatalogExaminationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogExaminationTypeItems",
                schema: "Catalog",
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
                    table.PrimaryKey("PK_CatalogExaminationTypeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogExaminationTypeItems_CatalogExaminationTypes_ExaminationTypeId",
                        column: x => x.ExaminationTypeId,
                        principalSchema: "Catalog",
                        principalTable: "CatalogExaminationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogExaminationTypeItems_ExaminationTypeId",
                schema: "Catalog",
                table: "CatalogExaminationTypeItems",
                column: "ExaminationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogExaminationTypeItems_ItemId",
                schema: "Catalog",
                table: "CatalogExaminationTypeItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogExaminationTypes_Code",
                schema: "Catalog",
                table: "CatalogExaminationTypes",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Examinations].[ExaminationTypes]', N'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM [Catalog].[CatalogExaminationTypes])
BEGIN
    INSERT INTO [Catalog].[CatalogExaminationTypes]
        ([Id],[Code],[Name],[Modality],[BodyPart],[StandardDurationMinutes],[Price],
         [RequiresPreparation],[RequiresConsent],[IsActive],
         [CreatedAt],[CreatedBy],[LastModifiedAt],[LastModifiedBy],[IsDeleted],[DeletedAt],[DeletedBy])
    SELECT [Id],[Code],[Name],[Modality],[BodyPart],[StandardDurationMinutes],[Price],
           [RequiresPreparation],[RequiresConsent],[IsActive],
           [CreatedAt],[CreatedBy],[LastModifiedAt],[LastModifiedBy],[IsDeleted],[DeletedAt],[DeletedBy]
    FROM [Examinations].[ExaminationTypes];

    INSERT INTO [Catalog].[CatalogExaminationTypeItems]
        ([Id],[ExaminationTypeId],[ItemId],[Quantity],[IsContrast],[IsRequired],[Notes])
    SELECT [Id],[ExaminationTypeId],[ItemId],[Quantity],[IsContrast],[IsRequired],[Notes]
    FROM [Examinations].[ExaminationTypeItems];
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogExaminationTypeItems",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "CatalogExaminationTypes",
                schema: "Catalog");
        }
    }
}
