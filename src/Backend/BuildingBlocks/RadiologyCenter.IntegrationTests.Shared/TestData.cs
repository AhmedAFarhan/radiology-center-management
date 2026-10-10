using System.Net.Http.Json;
using System.Text.Json;

namespace RadiologyCenter.IntegrationTests.Shared;

public static class TestData
{
    public static async Task<Guid> ExaminationAsync(HttpClient client, Guid patientId)
    {
        var typeId = await CreateAsync(client, "api/catalog/examination-types", new
        {
            Name = "Integration XRay " + Guid.NewGuid().ToString("N"), Modality = "XRay", BodyPart = "Chest", Price = 100m, StandardDurationMinutes = 30
        });
        return await CreateAsync(client, "api/examinations", new
        {
            PatientId = patientId, ExaminationTypeId = typeId, ClinicalIndication = "Integration indication", Priority = "Routine"
        });
    }
    public static Task<Guid> PatientAsync(HttpClient client) => CreateAsync(client, "api/patients", new
    {
        FullName = "Integration Test Patient", Gender = "Male", PhoneNumber = "01012345678", DateOfBirth = new DateTime(1990, 1, 1)
    });

    public static Task<Guid> StaffAsync(HttpClient client, string position) => CreateAsync(client, "api/resources/staff", new
    {
        UserId = Guid.NewGuid(), FullName = "Integration Clinical Staff", Position = position,
        PhoneNumber = "01012345678", HireDate = DateTime.UtcNow.Date.AddYears(-1)
    });

    public static Task<Guid> EquipmentAsync(HttpClient client, string modality = "XRay") => CreateAsync(client, "api/resources/equipment", new
    {
        Name = "Integration Equipment " + Guid.NewGuid().ToString("N"), Modality = modality
    });

    private static async Task<Guid> CreateAsync(HttpClient client, string path, object body)
    {
        using var response = await client.PostAsJsonAsync(path, body);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Fixture creation failed ({path}): {await response.Content.ReadAsStringAsync()}");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("data").GetProperty("id").GetGuid();
    }
}
