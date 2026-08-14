using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DropExaminationTypeItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogExaminationTypeItems",
                schema: "Catalog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogExaminationTypeItems",
                schema: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExaminationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsContrast = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
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
        }
    }
}
