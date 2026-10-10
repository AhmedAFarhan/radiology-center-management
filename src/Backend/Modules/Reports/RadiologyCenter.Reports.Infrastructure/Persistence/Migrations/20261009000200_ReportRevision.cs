using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RadiologyCenter.Reports.Infrastructure.Persistence.Migrations;

[DbContext(typeof(ReportsDbContext))]
[Migration("20261009000200_ReportRevision")]
public sealed class ReportRevision : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) =>
        migrationBuilder.AddColumn<int>(name: "Revision", table: "RadiologyReports", type: "int", schema: "Reports", nullable: false, defaultValue: 0);

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropColumn("Revision", "RadiologyReports", "Reports");
}
