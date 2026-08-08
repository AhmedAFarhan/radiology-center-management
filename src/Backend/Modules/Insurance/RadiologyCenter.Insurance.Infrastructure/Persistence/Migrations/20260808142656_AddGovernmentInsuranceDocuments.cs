using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Insurance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGovernmentInsuranceDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGovernment",
                schema: "Insurance",
                table: "PreAuthorizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGovernment",
                schema: "Insurance",
                table: "InsurancePolicies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PreAuthorizationDocuments",
                schema: "Insurance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreAuthorizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StoredPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreAuthorizationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreAuthorizationDocuments_PreAuthorizations_PreAuthorizationId",
                        column: x => x.PreAuthorizationId,
                        principalSchema: "Insurance",
                        principalTable: "PreAuthorizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreAuthorizationDocuments_PreAuthorizationId",
                schema: "Insurance",
                table: "PreAuthorizationDocuments",
                column: "PreAuthorizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreAuthorizationDocuments",
                schema: "Insurance");

            migrationBuilder.DropColumn(
                name: "IsGovernment",
                schema: "Insurance",
                table: "PreAuthorizations");

            migrationBuilder.DropColumn(
                name: "IsGovernment",
                schema: "Insurance",
                table: "InsurancePolicies");
        }
    }
}
