using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.IntegrationTests.Shared;
using Xunit;

namespace RadiologyCenter.Clinic.IntegrationTests;

[Collection("Direct SQL")]
public sealed class ClinicalWorkflowTests(DirectSqlFixture fixture)
{
    [Fact]
    public async Task NonAdministratorCanCompletePaidExaminationsAndSaveReportsAcrossClients()
    {
        var codes=new List<string>();
        await using(var db=new SqlConnection(fixture.AdminConnection))
        {
            await db.OpenAsync(); await using var command=db.CreateCommand();
            command.CommandText="SELECT Code FROM [Identity].[AspNetPermissions] WHERE Code NOT LIKE 'users.%' AND Code NOT LIKE 'roles.%'";
            await using var reader=await command.ExecuteReaderAsync(); while(await reader.ReadAsync()) codes.Add(reader.GetString(0));
        }
        codes.Add("roles.read"); codes.Add("users.read");
        var account=await fixture.CreateAccountAsync(codes);
        await using var first=await ClinicRuntime.StartAsync(DirectSqlFixture.Configuration(fixture.Account(account.User,account.Password)));
        await first.CheckReadinessAsync();
        await using var second=await ClinicRuntime.StartAsync(DirectSqlFixture.Configuration(fixture.Account(account.User,account.Password)));
        using var client=CustomWebApplicationFactory.CreateClientFor(first);
        using var other=CustomWebApplicationFactory.CreateClientFor(second);
        var patient=await TestData.PatientAsync(client);
        var exam=await TestData.ExaminationAsync(client,patient);
        var radiologist=await TestData.StaffAsync(client,"Radiologist");
        var technician=await TestData.StaffAsync(client,"Technician");
        var equipment=await TestData.EquipmentAsync(client);
        await Success(client.PostAsJsonAsync("api/inventory/items",new { name="Shared Stock "+Guid.NewGuid().ToString("N"),category="Consumable",unit="Piece" }));
        var cash=await Success(client.PostAsJsonAsync("api/cash/sessions",new { openingFloat=0 }));
        var cashId=cash.GetProperty("data").GetProperty("id").GetGuid();
        await Success(client.PutAsJsonAsync($"api/examinations/{exam}/staff",new { radiologistId=radiologist,technicianId=technician,equipmentId=equipment }));
        await Success(client.PostAsJsonAsync($"api/examinations/{exam}/payments",new { amount=100 }));
        await Success(client.PostAsJsonAsync($"api/examinations/{exam}/check-in",new { }));
        await Success(client.PostAsJsonAsync($"api/examinations/{exam}/start",new { }));
        await Success(client.PostAsJsonAsync($"api/examinations/{exam}/complete",new { }));
        var examination=await Success(other.GetAsync($"api/examinations/{exam}"));
        Assert.Equal("Completed",examination.GetProperty("data").GetProperty("statusKey").GetString());
        var report=await Success(client.PostAsJsonAsync("api/reports",new { examinationId=exam,patientId=patient,radiologistId=radiologist }));
        var reportId=report.GetProperty("data").GetProperty("id").GetGuid();
        await Success(client.PutAsJsonAsync($"api/reports/{reportId}/sections",new { sectionType="Findings",title="Findings",body="Committed shared report",expectedRevision=0 }));
        var conflict=await other.PutAsJsonAsync($"api/reports/{reportId}/sections",new { sectionType="Findings",title="Findings",body="Stale report",expectedRevision=0 });
        Assert.Equal(System.Net.HttpStatusCode.Conflict,conflict.StatusCode);
        Assert.Contains("Committed shared report",(await Success(other.GetAsync($"api/reports/{reportId}"))).ToString());
        var patientSummary=await Success(client.GetAsync($"api/patients/{patient}"));
        var patientCode=patientSummary.GetProperty("data").GetProperty("patientCode").GetString();
        for(var i=0;i<3;i++)
        {
            var next=await TestData.ExaminationAsync(client,patient);
            await Success(client.PostAsJsonAsync($"api/examinations/{next}/check-in",new { }));
        }
        var page1=await Success(client.PostAsJsonAsync("api/examinations/reading-queue",new { pageNumber=1,pageSize=2,search=patientCode }));
        var page2=await Success(other.PostAsJsonAsync("api/examinations/reading-queue",new { pageNumber=2,pageSize=2,search=patientCode }));
        Assert.Equal(4,page1.GetProperty("data").GetProperty("totalCount").GetInt32());
        var ids=page1.GetProperty("data").GetProperty("items").EnumerateArray().Concat(page2.GetProperty("data").GetProperty("items").EnumerateArray()).Select(e=>e.GetProperty("id").GetGuid()).ToArray();
        Assert.Equal(4,ids.Distinct().Count());
        Assert.All(page1.GetProperty("data").GetProperty("items").EnumerateArray(),e=>Assert.Contains("Integration",e.GetProperty("patientName").GetString()));
        var permissions=await Success(client.GetAsync("api/permissions")); Assert.True(permissions.GetProperty("data").GetArrayLength()>0);
        // Every collection operation is exercised using the same production SQL role, not a database owner.
        var collectionRoutes=typeof(ClinicDispatcher).Assembly.GetTypes().SelectMany(t=>t.GetMethods())
            .SelectMany(m=>m.GetCustomAttributes(typeof(ClinicOperationAttribute),false).Cast<ClinicOperationAttribute>())
            .Where(a=>a.Kind=="POST" && a.Name.EndsWith("/all")).Select(a=>a.Name).Distinct();
        foreach(var route in collectionRoutes) await Success(client.PostAsJsonAsync("api/"+route,new { pageNumber=1,pageSize=10 }));
        await Success(client.PostAsJsonAsync($"api/cash/sessions/{cashId}/close",new { countedTotal=100 }));
    }

    [Fact]
    public async Task RemovingRolePermissionsRevokesAccessOnExistingConnections()
    {
        var account=await fixture.CreateAccountAsync(new[]{"patients.read","patients.create"});
        await using var runtime=await ClinicRuntime.StartAsync(DirectSqlFixture.Configuration(fixture.Account(account.User,account.Password)));
        Assert.True((await runtime.Dispatcher.DispatchAsync(new("POST","patients/all","{}"))).IsSuccess);
        await using var admin=new SqlConnection(fixture.AdminConnection); await admin.OpenAsync();
        await DirectSqlFixture.Execute(admin,$"DELETE [Identity].[RolePermissions] WHERE RoleId='{account.RoleId}'");
        Assert.Equal(403,(await runtime.Dispatcher.DispatchAsync(new("POST","patients/all","{}"))).StatusCode);
        await using var raw=new SqlConnection(fixture.Account(account.User,account.Password)); await raw.OpenAsync();
        await using var query=raw.CreateCommand(); query.CommandText="SELECT COUNT(*) FROM [Patients].[Patients]";
        Assert.Equal(0,(int)(await query.ExecuteScalarAsync())!);
    }

    private static async Task<JsonElement> Success(Task<HttpResponseMessage> action)
    {
        using var response=await action; var content=await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode,$"{response.RequestMessage?.RequestUri}: {content}");
        return JsonDocument.Parse(content).RootElement.Clone();
    }
}
