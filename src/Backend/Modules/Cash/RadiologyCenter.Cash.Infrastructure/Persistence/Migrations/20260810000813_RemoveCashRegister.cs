using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologyCenter.Cash.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCashRegister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashRegisters",
                schema: "Cash");

            migrationBuilder.DropIndex(
                name: "IX_CashSessions_RegisterId_Status",
                schema: "Cash",
                table: "CashSessions");

            migrationBuilder.DropIndex(
                name: "IX_CashSessions_UserId_Status",
                schema: "Cash",
                table: "CashSessions");

            migrationBuilder.DropColumn(
                name: "RegisterId",
                schema: "Cash",
                table: "CashSessions");

            migrationBuilder.CreateIndex(
                name: "IX_CashSessions_Status",
                schema: "Cash",
                table: "CashSessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CashSessions_UserId",
                schema: "Cash",
                table: "CashSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CashSessions_Status",
                schema: "Cash",
                table: "CashSessions");

            migrationBuilder.DropIndex(
                name: "IX_CashSessions_UserId",
                schema: "Cash",
                table: "CashSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "RegisterId",
                schema: "Cash",
                table: "CashSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CashRegisters",
                schema: "Cash",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashRegisters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashSessions_RegisterId_Status",
                schema: "Cash",
                table: "CashSessions",
                columns: new[] { "RegisterId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CashSessions_UserId_Status",
                schema: "Cash",
                table: "CashSessions",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisters_Name",
                schema: "Cash",
                table: "CashRegisters",
                column: "Name",
                unique: true);
        }
    }
}
