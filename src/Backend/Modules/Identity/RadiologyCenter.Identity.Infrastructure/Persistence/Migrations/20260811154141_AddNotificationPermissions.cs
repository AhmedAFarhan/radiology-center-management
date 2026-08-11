using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationPermissions : Migration
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
                    { new Guid("43f1ae75-c3d8-8ea4-22ff-5dbb09ce694b"), "notifications.templates.manage", "Create, update, activate, deactivate and delete notification templates", "Notifications", "Manage Notification Templates" },
                    { new Guid("696b9a80-b38c-c946-53fc-09f235ea0525"), "notifications.messages.read", "View the notification message ledger", "Notifications", "Read Notification Messages" },
                    { new Guid("a70e2265-94ef-392b-8f49-b6eee6d7c033"), "notifications.messages.send", "Send SMS, email and push notifications", "Notifications", "Send Notifications" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("43f1ae75-c3d8-8ea4-22ff-5dbb09ce694b"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("696b9a80-b38c-c946-53fc-09f235ea0525"));

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "AspNetPermissions",
                keyColumn: "Id",
                keyValue: new Guid("a70e2265-94ef-392b-8f49-b6eee6d7c033"));
        }
    }
}
