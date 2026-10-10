using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace RadiologyCenter.Clinic.Security;

public static class DesktopSecurityInstaller
{
    public const int SchemaVersion = 1;
    public static async Task InstallAsync(SqlConnection connection, string loginPrefix, CancellationToken ct = default)
    {
        // Fail closed while replacing schema-bound policies: remove grants before dropping a policy.
        await ExecuteAsync(connection,"""
            DECLARE @sql nvarchar(max)=N'';
            SELECT @sql=@sql+N'REVOKE SELECT,INSERT,UPDATE,DELETE ON '+QUOTENAME(s.name)+N'.'+QUOTENAME(t.name)+N' FROM [EGcareClient];'
                FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id WHERE s.name NOT IN ('dbo','Wolverine','Security');
            IF DATABASE_PRINCIPAL_ID('EGcareClient') IS NOT NULL EXEC(@sql);
            SET @sql=N'';
            SELECT @sql=@sql+N'DROP SECURITY POLICY '+QUOTENAME(s.name)+N'.'+QUOTENAME(p.name)+N';'
                FROM sys.security_policies p JOIN sys.schemas s ON s.schema_id=p.schema_id WHERE s.name='Security' AND p.name LIKE 'Desktop[_]%';
            EXEC(@sql);
            IF OBJECT_ID('Security.CanWriteCashEntry') IS NOT NULL DROP FUNCTION [Security].[CanWriteCashEntry];
            IF OBJECT_ID('Security.CanWriteCashSession') IS NOT NULL DROP FUNCTION [Security].[CanWriteCashSession];
            IF OBJECT_ID('Security.CanChange') IS NOT NULL DROP FUNCTION [Security].[CanChange];
            IF OBJECT_ID('Security.CanAccess') IS NOT NULL DROP FUNCTION [Security].[CanAccess];
            IF OBJECT_ID('Security.DesktopSchema') IS NOT NULL UPDATE [Security].[DesktopSchema] SET Version=0 WHERE Id=1;
            """,ct);
        using var stream = typeof(DesktopSecurityInstaller).Assembly.GetManifestResourceStream("RadiologyCenter.Clinic.Security.DesktopSecurity.sql")!;
        var sql = await new StreamReader(stream).ReadToEndAsync(ct);
        foreach (var batch in Regex.Split(sql, @"^GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
            if (!string.IsNullOrWhiteSpace(batch)) await ExecuteAsync(connection,batch,ct);
        await using (var command=connection.CreateCommand())
        {
            command.CommandText="IF NOT EXISTS(SELECT 1 FROM [Security].[DesktopSchema] WHERE Id=1) INSERT [Security].[DesktopSchema](Id,Version,LoginPrefix) VALUES (1,0,@prefix);";
            command.Parameters.AddWithValue("@version",SchemaVersion); command.Parameters.AddWithValue("@prefix",loginPrefix);
            await command.ExecuteNonQueryAsync(ct);
        }
        var tables=new List<(string Schema,string Table,bool SoftDelete)>();
        await using (var command=connection.CreateCommand())
        {
            command.CommandText="SELECT SCHEMA_NAME(t.schema_id),t.name,CAST(CASE WHEN COL_LENGTH(QUOTENAME(SCHEMA_NAME(t.schema_id))+'.'+QUOTENAME(t.name),'IsDeleted') IS NULL THEN 0 ELSE 1 END AS bit) FROM sys.tables t WHERE t.is_ms_shipped=0";
            await using var reader=await command.ExecuteReaderAsync(ct);
            while(await reader.ReadAsync(ct)) tables.Add((reader.GetString(0),reader.GetString(1),reader.GetBoolean(2)));
        }
        foreach (var table in tables)
        {
            var rules=Rules(table.Schema,table.Table);
            if (rules is null) continue; // Unknown and transport tables receive no privileges.
            var name=Quote(table.Schema)+"."+Quote(table.Table);
            var policy="[Security]."+Quote("Desktop_"+table.Schema+"_"+table.Table);
            var ownerOnly=table.Schema=="Identity"?1:0;
            var readOwnerOnly=table.Schema=="Identity" && table.Table is "AspNetPermissions" or "PermissionTranslations"?0:ownerOnly;
            var read=Literal(string.Join(',',new[]{rules.Value.Read,rules.Value.Create,rules.Value.Update,rules.Value.Delete}.Where(x=>x.Length>0)));
            var create=Literal(rules.Value.Create); var update=Literal(rules.Value.Update); var delete=Literal(rules.Value.Delete);
            var deleted=table.SoftDelete?"[IsDeleted]":"0";
            var insertPredicate=$"[Security].[CanAccess]({create},{ownerOnly})";
            var updatePredicate=$"[Security].[CanChange]({update},{delete},{deleted},{ownerOnly})";
            var beforeUpdate="";
            if(table.Schema=="Cash" && table.Table=="CashSessions")
            {
                insertPredicate=$"[Security].[CanWriteCashSession]({create},[UserId],0)";
                updatePredicate=$"[Security].[CanWriteCashSession]({update},[UserId],1)";
                beforeUpdate=$"ADD BLOCK PREDICATE {updatePredicate} ON {name} BEFORE UPDATE,";
            }
            else if(table.Schema=="Cash" && table.Table is "CashEntries" or "CashHandovers")
                insertPredicate=$"[Security].[CanWriteCashEntry]({create},[CashSessionId],{(table.Table=="CashEntries"?1:0)})";
            await ExecuteAsync(connection,$"""
                IF EXISTS(SELECT 1 FROM sys.security_policies WHERE object_id=OBJECT_ID(N'{policy}')) DROP SECURITY POLICY {policy};
                CREATE SECURITY POLICY {policy}
                    ADD FILTER PREDICATE [Security].[CanAccess]({read},{readOwnerOnly}) ON {name},
                    ADD BLOCK PREDICATE {insertPredicate} ON {name} AFTER INSERT,
                    {beforeUpdate}
                    ADD BLOCK PREDICATE {updatePredicate} ON {name} AFTER UPDATE,
                    ADD BLOCK PREDICATE [Security].[CanAccess]({delete},{ownerOnly}) ON {name} BEFORE DELETE
                    WITH(STATE=ON,SCHEMABINDING=ON);
                GRANT SELECT,INSERT,UPDATE,DELETE ON {name} TO [EGcareClient];
                """,ct);
        }
        foreach(var table in new[]{"AspNetUsers","AspNetRoles","AspNetUserRoles","RolePermissions","AspNetPermissions"})
            await ExecuteAsync(connection,$"CREATE OR ALTER TRIGGER [Identity].{Quote("DesktopAuthority_"+table)} ON [Identity].{Quote(table)} AFTER INSERT,UPDATE,DELETE AS BEGIN SET NOCOUNT ON; EXEC [Security].[RebuildAuthority]; END",ct);
        await ExecuteAsync(connection,"EXEC [Security].[RebuildAuthority]",ct);
        await using(var complete=connection.CreateCommand())
        {
            complete.CommandText="UPDATE [Security].[DesktopSchema] SET Version=@version,ContractHash=@hash WHERE Id=1";
            complete.Parameters.AddWithValue("@version",SchemaVersion); complete.Parameters.AddWithValue("@hash",ContractHash(tables.Select(t=>(t.Schema,t.Table))));
            await complete.ExecuteNonQueryAsync(ct);
        }
    }
    public static string ContractHash(IEnumerable<(string Schema,string Table)> tables)
    {
        using var stream=typeof(DesktopSecurityInstaller).Assembly.GetManifestResourceStream("RadiologyCenter.Clinic.Security.DesktopSecurity.sql")!;
        var source=new StreamReader(stream).ReadToEnd().Replace("\r\n","\n").Trim();
        var rules=tables.Distinct().Where(t=>Rules(t.Schema,t.Table) is not null).OrderBy(t=>t.Schema,StringComparer.Ordinal).ThenBy(t=>t.Table,StringComparer.Ordinal)
            .Select(t=>t.Schema+"."+t.Table+":"+Rules(t.Schema,t.Table)!.Value.ToString());
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(source+"\n"+string.Join('\n',rules))));
    }
    private static (string Read,string Create,string Update,string Delete)? Rules(string schema,string table)
    {
        string? prefix=schema switch
        {
            "Patients"=>"patients", "Examinations"=>"examinations", "Catalog"=>"examinations.types", "Reports"=>"reports",
            "ResourceManagement"=>table switch { "Staff"=>"resources.staff", "Equipment"=>"resources.equipment", "WorkShifts"=>"resources.shifts", "Leaves"=>"resources.leave", "ReferralDoctors"=>"resources.referrals", _=>null },
            "Inventory"=>table switch { "Items"=>"inventory.items", "Suppliers"=>"inventory.suppliers", "PurchaseOrders" or "PurchaseOrderItems"=>"inventory.purchase-orders", _=>"inventory.stock" },
            "Insurance"=>table switch { "InsuranceCompanies"=>"insurance.companies", "InsurancePolicies" or "PolicyDocuments"=>"insurance.policies", "PreAuthorizations" or "PreAuthorizationDocuments"=>"insurance.preauthorizations", _=>"insurance.claims" },
            "Identity"=>"identity.admin", _=>null
        };
        if(schema=="System") return table switch {
            "DesktopEvents"=>("",string.Join(',',RadiologyCenter.Identity.Domain.Permissions.All.Select(p=>p.Code)
                .Where(code=>!code.EndsWith(".read") && !code.EndsWith(".export")).OrderBy(code=>code,StringComparer.Ordinal)),"",""),
            "NumberSequences"=>("patients.create,examinations.create,inventory.purchase-orders.create,insurance.claims.create","patients.create,examinations.create,inventory.purchase-orders.create,insurance.claims.create","patients.create,examinations.create,inventory.purchase-orders.create,insurance.claims.create",""), _=>null };
        if(schema=="Cash") return table switch {
            "CashSessions"=>("cash.sessions.read,cash.sessions.close,cash.entries.add,cash.handovers.approve,examinations.update,analytics.read","cash.sessions.open","cash.sessions.close,cash.entries.add,examinations.update,cash.handovers.approve",""),
            "CashEntries"=>("cash.sessions.read,analytics.read","cash.entries.add,examinations.update,cash.sessions.close,cash.handovers.approve","",""),
            "CashHandovers"=>("cash.handovers.read,analytics.read","cash.sessions.close","cash.handovers.approve",""), _=>null };
        if(schema=="Notification") return table switch {
            "NotificationTemplates"=>("notifications.templates.manage","notifications.templates.manage","notifications.templates.manage","notifications.templates.manage"),
            "NotificationMessages"=>("notifications.messages.read","notifications.messages.send","notifications.messages.send",""), _=>null };
        if(schema=="Payroll") {
            var manage=table switch { "SalaryComponents"=>"payroll.salary-components.manage", "Salaries"=>"payroll.salary.manage", "AllowanceAssignments"=>"payroll.allowances.manage", "ExaminationFees" or "ReferralFees"=>"payroll.fees.manage", _=>"payroll.payruns.manage,payroll.payruns.run" };
            return ("payroll.read,analytics.read"+(table is "ExaminationFees" or "ReferralFees"?",examinations.perform,examinations.update":""),manage,manage,manage);
        }
        if(prefix is null) return null;
        if(schema=="Identity" && table is "AspNetPermissions" or "PermissionTranslations") return ("roles.read","identity.admin","identity.admin","identity.admin");
        if(table=="ExaminationTypeItems") return ("examinations.read,examinations.create,examinations.update,examinations.types.manage","examinations.types.manage","examinations.types.manage","examinations.types.manage");
        if(table=="DocumentContents") return ("insurance.policies.read,insurance.preauthorizations.read","insurance.policies.update,insurance.preauthorizations.attach-document","","");
        var read=prefix+".read,analytics.read";
        if(schema=="Catalog") read="examinations.types.manage,analytics.read";
        if(schema=="Inventory" && table=="Items") read+=",examinations.read,examinations.create,examinations.update,inventory.purchase-orders.create,inventory.purchase-orders.update";
        if(schema=="Patients") read+=",examinations.read,examinations.create,examinations.update,reports.read,insurance.policies.read,insurance.claims.read";
        if(schema=="Catalog" || schema=="ResourceManagement") read+=",examinations.read,examinations.create,examinations.update,reports.read,payroll.read";
        if(schema=="Examinations") read+=",reports.read,reports.create,reports.update,insurance.claims.read,insurance.claims.create,payroll.read";
        if(schema=="Reports") read+=",examinations.read";
        if(schema=="Insurance") read+=",examinations.read,examinations.create,examinations.update";
        var create=prefix+".create"; var update=prefix+".update"; var delete=prefix+".delete";
        if(schema=="Catalog") create=update=delete="examinations.types.manage";
        if(schema=="Examinations") update+=",examinations.perform,examinations.cancel";
        if(schema=="Reports" && table=="RadiologyReports") update+=",reports.delete";
        if(prefix=="inventory.stock") create=update="inventory.stock.issue,inventory.purchase-orders.update";
        if(table=="Claims" || table=="Settlements") { create+=",insurance.claims.settle"; update+=",insurance.claims.settle"; }
        if(table=="PolicyDocuments") create=update="insurance.policies.update";
        if(table=="PreAuthorizationDocuments") create=update="insurance.preauthorizations.attach-document";
        return (read,create,update,delete);
    }
    internal static string Quote(string name)=>"["+name.Replace("]","]]")+ "]";
    internal static string Literal(string value)=>"N'"+value.Replace("'","''")+"'";
    private static async Task ExecuteAsync(SqlConnection connection,string sql,CancellationToken ct)
    { await using var command=connection.CreateCommand(); command.CommandText=sql; command.CommandTimeout=120; await command.ExecuteNonQueryAsync(ct); }
}
