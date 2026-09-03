using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class ExaminationTypesTests : TestBase
{
    private const string BaseUrl = "api/catalog/examination-types";

    public ExaminationTypesTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_ValidData_ReturnsOk()
    {
        var command = new
        {
            Name = $"XRay Chest {Guid.NewGuid():N}",
            Modality = "XRay",
            BodyPart = "Chest",
            StandardDurationMinutes = 15,
            Price = 250m,
            RequiresPreparation = false,
            RequiresConsent = false
        };

        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Create_MissingName_ReturnsBadRequest()
    {
        var command = new
        {
            Name = "",
            Modality = "XRay",
            BodyPart = "Chest"
        };

        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_MissingModality_ReturnsBadRequest()
    {
        var command = new
        {
            Name = $"CT Head {Guid.NewGuid():N}",
            Modality = "",
            BodyPart = "Head"
        };

        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_MissingBodyPart_ReturnsBadRequest()
    {
        var command = new
        {
            Name = $"MRI Brain {Guid.NewGuid():N}",
            Modality = "MRI",
            BodyPart = ""
        };

        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_InvalidModality_ReturnsBadRequest()
    {
        var command = new
        {
            Name = $"Test {Guid.NewGuid():N}",
            Modality = "InvalidModality",
            BodyPart = "Chest"
        };

        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_NegativePrice_ReturnsBadRequest()
    {
        var command = new
        {
            Name = $"Test {Guid.NewGuid():N}",
            Modality = "XRay",
            BodyPart = "Chest",
            Price = -100m
        };

        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_NegativeDuration_ReturnsBadRequest()
    {
        var command = new
        {
            Name = $"Test {Guid.NewGuid():N}",
            Modality = "XRay",
            BodyPart = "Chest",
            StandardDurationMinutes = -5
        };

        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        var id = await CreateTestExaminationTypeAsync();
        var response = await Client.GetAsync($"{BaseUrl}/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationTypeDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{BaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllPaged_ReturnsOk()
    {
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ExaminationTypeDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Update_ValidData_ReturnsOk()
    {
        var id = await CreateTestExaminationTypeAsync();
        var command = new
        {
            ExaminationTypeId = id,
            Name = $"Updated CT {Guid.NewGuid():N}",
            Modality = "CT",
            BodyPart = "Abdomen",
            StandardDurationMinutes = 30,
            Price = 500m,
            RequiresPreparation = true,
            RequiresConsent = false
        };

        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{id}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new
        {
            ExaminationTypeId = fakeId,
            Name = $"Ghost {Guid.NewGuid():N}",
            Modality = "XRay",
            BodyPart = "Head"
        };

        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{fakeId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_DuplicateName_ReturnsConflict()
    {
        var name1 = $"Dup1_{Guid.NewGuid():N}";
        var name2 = $"Dup2_{Guid.NewGuid():N}";
        await CreateTestExaminationTypeAsync(name1, "XRay", "Chest");
        var id2 = await CreateTestExaminationTypeAsync(name2, "CT", "Abdomen");

        var command = new
        {
            ExaminationTypeId = id2,
            Name = name1,
            Modality = "CT",
            BodyPart = "Abdomen"
        };

        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{id2}", command);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Activate_ReturnsOk()
    {
        var id = await CreateTestExaminationTypeAsync();
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{id}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Deactivate_ReturnsOk()
    {
        var id = await CreateTestExaminationTypeAsync();
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{id}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ReturnsOk()
    {
        var id = await CreateTestExaminationTypeAsync();
        var response = await Client.DeleteAsync($"{BaseUrl}/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{BaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateThenGetById_VerifyData()
    {
        var name = $"VerifyData_{Guid.NewGuid():N}";
        var id = await CreateTestExaminationTypeAsync(name, "MRI", "Spine");

        var response = await Client.GetAsync($"{BaseUrl}/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationTypeDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(id);
        body.Data.Name.Should().Be(name);
        body.Data.Modality.Should().Be("MRI");
        body.Data.BodyPart.Should().Be("Spine");
        body.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateThenUpdateName_VerifyPersistence()
    {
        var originalName = $"Original_{Guid.NewGuid():N}";
        var id = await CreateTestExaminationTypeAsync(originalName, "XRay", "Chest");

        var newName = $"Renamed_{Guid.NewGuid():N}";
        var updateCommand = new
        {
            ExaminationTypeId = id,
            Name = newName,
            Modality = "XRay",
            BodyPart = "Chest"
        };
        await Client.PutAsJsonAsync($"{BaseUrl}/{id}", updateCommand);

        var getResponse = await Client.GetAsync($"{BaseUrl}/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ExaminationTypeDto>>();
        body!.Data!.Name.Should().Be(newName);
        body.Data.Name.Should().NotBe(originalName);
    }

    [Theory]
    [InlineData("XRay")]
    [InlineData("CT")]
    [InlineData("MRI")]
    [InlineData("Ultrasound")]
    [InlineData("Mammography")]
    [InlineData("Fluoroscopy")]
    [InlineData("DEXA")]
    [InlineData("Other")]
    public async Task Create_WithEachModalityType_ReturnsOk(string modality)
    {
        var command = new
        {
            Name = $"Modality_{modality}_{Guid.NewGuid():N}",
            Modality = modality,
            BodyPart = "Chest",
            StandardDurationMinutes = 20,
            Price = 300m
        };

        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task ExportExaminationTypes_ReturnsOk()
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
    public async Task ImportExaminationTypes_WithValidFile_ReturnsOk()
    {
        var fileContent = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
        var content = new MultipartFormDataContent();
        var fileContentObj = new ByteArrayContent(fileContent);
        fileContentObj.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        content.Add(fileContentObj, "File", "examination-types.xlsx");

        var response = await Client.PostAsync($"{BaseUrl}/import", content);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> CreateTestExaminationTypeAsync(
        string? name = null,
        string? modality = null,
        string? bodyPart = null)
    {
        var command = new
        {
            Name = name ?? $"TestExType_{Guid.NewGuid():N}",
            Modality = modality ?? "XRay",
            BodyPart = bodyPart ?? "Chest",
            StandardDurationMinutes = 15,
            Price = 250m,
            RequiresPreparation = false,
            RequiresConsent = false
        };

        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.EnsureSuccessStatusCode();

        var allResponse = await Client.PostAsJsonAsync($"{BaseUrl}/all",
            new { Pagination = new { PageNumber = 1, PageSize = 1 } });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ExaminationTypeDto>>>();
        return allBody!.Data!.Items.First().Id;
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

    private sealed class ExaminationTypeDto
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Modality { get; set; }
        public string? BodyPart { get; set; }
        public int StandardDurationMinutes { get; set; }
        public decimal Price { get; set; }
        public bool RequiresPreparation { get; set; }
        public bool RequiresConsent { get; set; }
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
