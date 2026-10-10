using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RadiologyCenter.Cash.Infrastructure.Persistence.Migrations;

[DbContext(typeof(CashDbContext))]
[Migration("20261009000100_OneOpenCashSessionPerUser")]
public sealed class OneOpenCashSessionPerUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF EXISTS (SELECT UserId FROM Cash.CashSessions WHERE Status = 1 AND IsDeleted = 0 GROUP BY UserId HAVING COUNT(*) > 1)
                THROW 51000, 'Reconcile duplicate open cash sessions before upgrading.', 1;
            """);
        migrationBuilder.DropIndex("IX_CashSessions_UserId", "CashSessions", "Cash");
        migrationBuilder.CreateIndex("IX_CashSessions_UserId", "CashSessions", "UserId", "Cash",
            unique: true, filter: "[Status] = 1 AND [IsDeleted] = 0");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("IX_CashSessions_UserId", "CashSessions", "Cash");
        migrationBuilder.CreateIndex("IX_CashSessions_UserId", "CashSessions", "UserId", "Cash");
    }
}
