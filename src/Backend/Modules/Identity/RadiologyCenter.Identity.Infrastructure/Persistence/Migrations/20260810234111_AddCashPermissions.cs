using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCashPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Identity",
                table: "AspNetPermissions",
                columns: new[] { "Id", "Code", "Description", "Group", "Name" },
                values: new object[,]
                {
                    { new Guid("0c53690f-9110-ae9b-7d60-0bac108eb5a6"), "cash.handovers.approve", "Approve completed cash handovers", "Cash", "Approve Cash Handovers" },
                    { new Guid("2664e39f-c5e0-e76a-3865-6c1517be65f6"), "cash.sessions.read", "View cash sessions and their entries", "Cash", "Read Cash Sessions" },
                    { new Guid("5b48738f-a618-d143-2a8b-c9873d909a90"), "cash.sessions.open", "Open a new cash session and record the opening float", "Cash", "Open Cash Sessions" },
                    { new Guid("c0796834-1791-85ec-5836-937d1ba0b51c"), "cash.entries.add", "Record cash movements in a cash session", "Cash", "Add Cash Entries" },
                    { new Guid("d699701c-3a24-f3fd-6e00-074c7f59cfbb"), "cash.sessions.close", "Close a cash session and reconcile a counted handover", "Cash", "Close Cash Sessions" },
                    { new Guid("e8040222-04b5-140f-c6dc-35bd1188ff10"), "cash.handovers.read", "View cash handovers and reconciliation details", "Cash", "Read Cash Handovers" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("0c53690f-9110-ae9b-7d60-0bac108eb5a6"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("2664e39f-c5e0-e76a-3865-6c1517be65f6"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("5b48738f-a618-d143-2a8b-c9873d909a90"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("c0796834-1791-85ec-5836-937d1ba0b51c"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("d699701c-3a24-f3fd-6e00-074c7f59cfbb"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("e8040222-04b5-140f-c6dc-35bd1188ff10"));
        }
    }
}
