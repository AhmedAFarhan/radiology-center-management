using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RadiologyCenter.Insurance.Infrastructure.Persistence.Migrations;
[DbContext(typeof(InsuranceDbContext))]
[Migration("20261009000400_SharedDocumentContents")]
public sealed class SharedDocumentContents : Migration
{
    protected override void Up(MigrationBuilder builder)=>builder.Sql("""
        CREATE TABLE [Insurance].[DocumentContents](Id uniqueidentifier PRIMARY KEY,
          Content varbinary(max) NOT NULL CHECK(DATALENGTH(Content)<=20971520));
        """);
    protected override void Down(MigrationBuilder builder)=>builder.DropTable("DocumentContents","Insurance");
}
