using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261009000300_DesktopEventJournal")]
public sealed class DesktopEventJournal : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        CREATE TABLE [System].[DesktopEvents] (
            Id bigint IDENTITY PRIMARY KEY,
            EventType nvarchar(200) NOT NULL,
            Payload nvarchar(max) NOT NULL,
            CreatedAt datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
            LoginName sysname NOT NULL DEFAULT ORIGINAL_LOGIN()
        );
        CREATE INDEX IX_DesktopEvents_CreatedAt ON [System].[DesktopEvents](CreatedAt);
        """);
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("DesktopEvents", "System");
}
