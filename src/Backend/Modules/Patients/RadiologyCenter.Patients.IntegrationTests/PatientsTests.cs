using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class PatientsTests : TestBase
{
    private const string BaseUrl = "api/patients";

    public PatientsTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_PatientWithValidData_ReturnsOk()
    {
        var command = new
        {
            FullName = "Ahmed Mohamed Ali",
            Gender = "Male",
            PhoneNumber = "01012345678",
            DateOfBirth = new DateTime(1990, 5, 15)
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.FullName.Should().Be("Ahmed Mohamed Ali");
        body.Data.Gender.Should().Be("Male");
        body.Data.PhoneNumber.Should().Be("01012345678");
        body.Data.PatientCode.Should().StartWith("PTN-");
    }

    [Fact]
    public async Task Create_MissingFullName_ReturnsBadRequest()
    {
        var command = new
        {
            FullName = "",
            Gender = "Male",
            PhoneNumber = "01012345678",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_MissingGender_ReturnsBadRequest()
    {
        var command = new
        {
            FullName = "Ahmed Mohamed",
            Gender = "",
            PhoneNumber = "01012345678",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_MissingPhoneNumber_ReturnsBadRequest()
    {
        var command = new
        {
            FullName = "Ahmed Mohamed",
            Gender = "Male",
            PhoneNumber = "",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_NoDateOfBirthOrAge_ReturnsBadRequest()
    {
        var command = new
        {
            FullName = "Ahmed Mohamed",
            Gender = "Male",
            PhoneNumber = "01012345678",
            DateOfBirth = (DateTime?)null,
            Age = (int?)null
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_InvalidEmail_ReturnsBadRequest()
    {
        var command = new
        {
            FullName = "Ahmed Mohamed",
            Gender = "Male",
            PhoneNumber = "01012345678",
            DateOfBirth = new DateTime(1990, 1, 1),
            Email = "not-an-email"
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_SingleName_ReturnsBadRequest()
    {
        var command = new
        {
            FullName = "Ahmed",
            Gender = "Male",
            PhoneNumber = "01012345678",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ExistingPatient_ReturnsOk()
    {
        var patientId = await CreateTestPatientAsync();
        var response = await Client.GetAsync($"{BaseUrl}/{patientId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(patientId);
    }

    [Fact]
    public async Task GetById_NonexistentPatient_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{BaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_ReturnsPagedResult()
    {
        var request = new
        {
            Pagination = new { PageNumber = 1, PageSize = 10 }
        };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<PatientDto>>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Update_PatientWithValidData_ReturnsOk()
    {
        var patientId = await CreateTestPatientAsync();
        var command = new
        {
            FullName = "Sara Ahmed Hassan",
            Gender = "Female",
            PhoneNumber = "01098765432",
            DateOfBirth = new DateTime(1985, 3, 20)
        };
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{patientId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_NonexistentPatient_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new
        {
            FullName = "Updated Name",
            Gender = "Male",
            PhoneNumber = "01012345678",
            DateOfBirth = new DateTime(1990, 1, 1)
        };
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{fakeId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Activate_Patient_ReturnsOk()
    {
        var patientId = await CreateTestPatientAsync();
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{patientId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Deactivate_Patient_ReturnsOk()
    {
        var patientId = await CreateTestPatientAsync();
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{patientId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_Patient_ReturnsOk()
    {
        var patientId = await CreateTestPatientAsync();
        var response = await Client.DeleteAsync($"{BaseUrl}/{patientId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_NonexistentPatient_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{BaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateThenGetById_VerifyDataMatches()
    {
        var patientId = await CreateTestPatientAsync();
        var response = await Client.GetAsync($"{BaseUrl}/{patientId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(patientId);
        body.Data.FullName.Should().Be("Ahmed Mohamed Ali");
        body.Data.Gender.Should().Be("Male");
        body.Data.PhoneNumber.Should().Be("01012345678");
        body.Data.DateOfBirth.Should().Be(new DateTime(1990, 5, 15));
        body.Data.IsActive.Should().BeTrue();
        body.Data.PatientCode.Should().StartWith("PTN-");
    }

    [Fact]
    public async Task CreateThenUpdate_VerifyChangesPersist()
    {
        var patientId = await CreateTestPatientAsync();
        var updateCommand = new
        {
            FullName = "Sara Ahmed Hassan",
            Gender = "Female",
            PhoneNumber = "01098765432",
            DateOfBirth = new DateTime(1985, 3, 20),
            Email = "sara@example.com",
            Address = "Cairo, Egypt"
        };
        var updateResponse = await Client.PutAsJsonAsync($"{BaseUrl}/{patientId}", updateCommand);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await Client.GetAsync($"{BaseUrl}/{patientId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await getResponse.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.FullName.Should().Be("Sara Ahmed Hassan");
        body.Data.Gender.Should().Be("Female");
        body.Data.PhoneNumber.Should().Be("01098765432");
        body.Data.Email.Should().Be("sara@example.com");
        body.Data.Address.Should().Be("Cairo, Egypt");
    }

    [Fact]
    public async Task DeactivateThenActivate_VerifyIsActiveToggles()
    {
        var patientId = await CreateTestPatientAsync();

        var deactivateResponse = await Client.PostAsJsonAsync($"{BaseUrl}/{patientId}/deactivate", new { });
        deactivateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getAfterDeactivate = await Client.GetAsync($"{BaseUrl}/{patientId}");
        var bodyAfterDeactivate = await getAfterDeactivate.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        bodyAfterDeactivate!.Data!.IsActive.Should().BeFalse();

        var activateResponse = await Client.PostAsJsonAsync($"{BaseUrl}/{patientId}/activate", new { });
        activateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getAfterActivate = await Client.GetAsync($"{BaseUrl}/{patientId}");
        var bodyAfterActivate = await getAfterActivate.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        bodyAfterActivate!.Data!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ExportPatients_ReturnsOk()
    {
        var request = new
        {
            Pagination = new { PageNumber = 1, PageSize = 10 }
        };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/export", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.Should().NotBeNull();
    }

    [Fact]
    public async Task GetImportTemplate_ReturnsOk()
    {
        var response = await Client.GetAsync($"{BaseUrl}/import-template");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.Should().NotBeNull();
    }

    [Fact]
    public async Task ImportPatients_WithValidFile_ReturnsOk()
    {
        var fileContent = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
        var content = new MultipartFormDataContent();
        var fileContentObj = new ByteArrayContent(fileContent);
        fileContentObj.Headers.ContentType = new("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        content.Add(fileContentObj, "File", "patients.xlsx");

        var response = await Client.PostAsync($"{BaseUrl}/import", content);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithAllOptionalFields_VerifyTheyPersist()
    {
        var command = new
        {
            FullName = "Fatma Ali Hassan",
            Gender = "Female",
            PhoneNumber = "01055556666",
            DateOfBirth = new DateTime(1988, 7, 10),
            Email = "fatma@example.com",
            Address = "123 Nile Street, Cairo",
            NationalId = "29807101234567",
            BloodType = "A+",
            Allergies = "Penicillin",
            MedicalHistory = "No chronic conditions"
        };
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, command);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var createBody = await createResponse.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        var patientId = createBody!.Data!.Id;

        var getResponse = await Client.GetAsync($"{BaseUrl}/{patientId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await getResponse.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.FullName.Should().Be("Fatma Ali Hassan");
        body.Data.Email.Should().Be("fatma@example.com");
        body.Data.Address.Should().Be("123 Nile Street, Cairo");
        body.Data.NationalId.Should().Be("29807101234567");
        body.Data.BloodType.Should().Be("A+");
        body.Data.Allergies.Should().Be("Penicillin");
        body.Data.MedicalHistory.Should().Be("No chronic conditions");
    }

    private async Task<Guid> CreateTestPatientAsync()
    {
        var command = new
        {
            FullName = "Ahmed Mohamed Ali",
            Gender = "Male",
            PhoneNumber = "01012345678",
            DateOfBirth = new DateTime(1990, 5, 15)
        };
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, command);
        createResponse.EnsureSuccessStatusCode();
        var body = await createResponse.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        return body!.Data!.Id;
    }

    private sealed class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public ApiErrorDto? Error { get; set; }
    }

    private sealed class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public ApiErrorDto? Error { get; set; }
    }

    private sealed class ApiErrorDto
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }

    private sealed class PatientDto
    {
        public Guid Id { get; set; }
        public string? PatientCode { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? NationalId { get; set; }
        public string? BloodType { get; set; }
        public string? Allergies { get; set; }
        public string? MedicalHistory { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    private sealed class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
