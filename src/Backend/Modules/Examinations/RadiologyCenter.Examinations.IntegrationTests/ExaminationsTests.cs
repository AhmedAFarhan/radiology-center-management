using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class ExaminationsTests : TestBase
{
    private const string BaseUrl = "api/examinations";

    public ExaminationsTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_ValidData_ReturnsOk()
    {
        var patientId = Guid.NewGuid();
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var command = new
        {
            PatientId = patientId,
            ExaminationTypeId = examinationTypeId,
            ClinicalIndication = "Chest pain evaluation",
            Priority = "Routine",
            Discount = 0m,
            IsDiscountPercentage = false,
            Paid = 0m
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_MissingPatientId_ReturnsBadRequest()
    {
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var command = new
        {
            ExaminationTypeId = examinationTypeId,
            ClinicalIndication = "Test indication",
            Priority = "Routine"
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_MissingExaminationTypeId_ReturnsBadRequest()
    {
        var patientId = Guid.NewGuid();
        var command = new
        {
            PatientId = patientId,
            ClinicalIndication = "Test indication",
            Priority = "Routine"
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_MissingClinicalIndication_ReturnsBadRequest()
    {
        var patientId = Guid.NewGuid();
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var command = new
        {
            PatientId = patientId,
            ExaminationTypeId = examinationTypeId,
            Priority = "Routine"
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_MissingPriority_ReturnsBadRequest()
    {
        var patientId = Guid.NewGuid();
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var command = new
        {
            PatientId = patientId,
            ExaminationTypeId = examinationTypeId,
            ClinicalIndication = "Test indication"
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_NegativeDiscount_ReturnsBadRequest()
    {
        var patientId = Guid.NewGuid();
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var command = new
        {
            PatientId = patientId,
            ExaminationTypeId = examinationTypeId,
            ClinicalIndication = "Test indication",
            Priority = "Routine",
            Discount = -10m,
            IsDiscountPercentage = false,
            Paid = 0m
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var response = await Client.GetAsync($"{BaseUrl}/{examId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(examId);
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
        await CreateTestExaminationAsync();
        var request = new { PageNumber = 1, PageSize = 10 };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ExaminationListItemDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ScheduleExamination_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var command = new { ScheduledAt = DateTime.UtcNow.AddHours(2) };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{examId}/schedule", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CheckIn_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var response = await Client.PostAsync($"{BaseUrl}/{examId}/check-in", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Start_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync(status: "CheckedIn", paid: 0m);
        var response = await Client.PostAsync($"{BaseUrl}/{examId}/start", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Complete_ReturnsOk()
    {
        var radiologistId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var examId = await CreateTestExaminationAsync(
            status: "CheckedIn",
            paid: 0m,
            radiologistId: radiologistId,
            technicianId: technicianId,
            equipmentId: equipmentId);

        await Client.PostAsync($"{BaseUrl}/{examId}/start", null);
        var response = await Client.PostAsync($"{BaseUrl}/{examId}/complete", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Cancel_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{examId}/cancel", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Cancel_WithReason_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var command = new { Reason = "Patient request" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{examId}/cancel", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponse = await Client.GetAsync($"{BaseUrl}/{examId}");
        var body = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        body!.Data!.CancellationReason.Should().Be("Patient request");
    }

    [Fact]
    public async Task RecordPayment_ValidAmount_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync(paid: 0m);
        var command = new { Amount = 50m, Description = "Partial payment" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{examId}/payments", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RecordPayment_ExceedsRemaining_ReturnsBadRequest()
    {
        var examId = await CreateTestExaminationAsync(paid: 0m);
        var command = new { Amount = 999999m, Description = "Oversized payment" };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{examId}/payments", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddItem_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var itemId = await GetOrCreateItemAsync();
        var command = new
        {
            ItemId = itemId,
            Quantity = 1,
            IsContrast = false,
            IsRequired = false,
            Notes = (string?)null
        };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{examId}/items", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateThenGetById_VerifyData()
    {
        var patientId = Guid.NewGuid();
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var clinicalIndication = "Verification test";
        var createCommand = new
        {
            PatientId = patientId,
            ExaminationTypeId = examinationTypeId,
            ClinicalIndication = clinicalIndication,
            Priority = "Urgent",
            Discount = 0m,
            IsDiscountPercentage = false,
            Paid = 0m
        };
        var createResponse = await Client.PostAsJsonAsync(BaseUrl, createCommand);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var createBody = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        var examId = createBody!.Data!.Id;

        var getResponse = await Client.GetAsync($"{BaseUrl}/{examId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        getBody!.Data!.PatientId.Should().Be(patientId);
        getBody.Data.ExaminationTypeId.Should().Be(examinationTypeId);
        getBody.Data.ClinicalIndication.Should().Be(clinicalIndication);
        getBody.Data.Priority.Should().Be("Urgent");
    }

    [Fact]
    public async Task FullLifecycle_Create_CheckIn_Start_Complete()
    {
        var radiologistId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();

        var examId = await CreateTestExaminationAsync(
            paid: 0m,
            radiologistId: radiologistId,
            technicianId: technicianId,
            equipmentId: equipmentId);

        var checkInResponse = await Client.PostAsync($"{BaseUrl}/{examId}/check-in", null);
        checkInResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var startResponse = await Client.PostAsync($"{BaseUrl}/{examId}/start", null);
        startResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var completeResponse = await Client.PostAsync($"{BaseUrl}/{examId}/complete", null);
        completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var verifyResponse = await Client.GetAsync($"{BaseUrl}/{examId}");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await verifyResponse.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        body!.Data!.Status.Should().Be("Completed");
        body.Data.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Cancel_NonexistentExamination_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{fakeId}/cancel", new { });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddItem_NonexistentExamination_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var itemId = await GetOrCreateItemAsync();
        var command = new
        {
            ItemId = itemId,
            Quantity = 1,
            IsContrast = false,
            IsRequired = false
        };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{fakeId}/items", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RecordPayment_NonexistentExamination_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { Amount = 10m };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{fakeId}/payments", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RecordPayment_ZeroAmount_ReturnsBadRequest()
    {
        var examId = await CreateTestExaminationAsync(paid: 0m);
        var command = new { Amount = 0m };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{examId}/payments", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Start_WithOutstandingBalance_ReturnsError()
    {
        var examId = await CreateTestExaminationAsync(status: "CheckedIn");
        var response = await Client.PostAsync($"{BaseUrl}/{examId}/start", null);
        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Book_ValidData_ReturnsOk()
    {
        var patientId = Guid.NewGuid();
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var command = new
        {
            PatientId = patientId,
            ExaminationTypeId = examinationTypeId,
            ScheduledAt = DateTime.UtcNow.AddDays(1),
            Priority = "Routine",
            ClinicalIndication = "Book test"
        };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/book", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    private async Task<Guid> CreateTestExaminationAsync(
        string? status = null,
        decimal? paid = null,
        Guid? radiologistId = null,
        Guid? technicianId = null,
        Guid? equipmentId = null)
    {
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var command = new
        {
            PatientId = Guid.NewGuid(),
            ExaminationTypeId = examinationTypeId,
            ClinicalIndication = "Test indication",
            Priority = "Routine",
            Discount = 0m,
            IsDiscountPercentage = false,
            Paid = paid ?? 0m,
            RadiologistId = radiologistId,
            TechnicianId = technicianId,
            EquipmentId = equipmentId,
            Status = status
        };
        var response = await Client.PostAsJsonAsync(BaseUrl, command);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        return body!.Data!.Id;
    }

    private async Task<Guid> GetOrCreateExaminationTypeAsync()
    {
        var response = await Client.PostAsJsonAsync("api/examination-types/all", new { PageNumber = 1, PageSize = 1 });
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ExaminationTypeListItemDto>>>();
            if (body?.Data?.Items.Count > 0)
                return body.Data.Items.First().Id;
        }

        var createTypeCommand = new
        {
            Name = $"TestType_{Guid.NewGuid():N}",
            Code = $"TST_{Guid.NewGuid():N}",
            Modality = "X-Ray",
            BodyPart = "Chest",
            StandardDurationMinutes = 30,
            Price = 100m,
            RequiresPreparation = false,
            RequiresConsent = false,
            RequiresContrast = false
        };
        var createResponse = await Client.PostAsJsonAsync("api/examination-types", createTypeCommand);
        createResponse.EnsureSuccessStatusCode();

        var allResponse = await Client.PostAsJsonAsync("api/examination-types/all", new { PageNumber = 1, PageSize = 1 });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ExaminationTypeListItemDto>>>();
        return allBody!.Data!.Items.First().Id;
    }

    private async Task<Guid> GetOrCreateItemAsync()
    {
        var response = await Client.PostAsJsonAsync("api/items/all", new { PageNumber = 1, PageSize = 1 });
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ItemListItemDto>>>();
            if (body?.Data?.Items.Count > 0)
                return body.Data.Items.First().Id;
        }

        var createCommand = new
        {
            Name = $"TestItem_{Guid.NewGuid():N}",
            Code = $"ITM_{Guid.NewGuid():N}",
            Unit = "unit",
            UnitPrice = 10m
        };
        var createResponse = await Client.PostAsJsonAsync("api/items", createCommand);
        createResponse.EnsureSuccessStatusCode();

        var allResponse = await Client.PostAsJsonAsync("api/items/all", new { PageNumber = 1, PageSize = 1 });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ItemListItemDto>>>();
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

    private sealed class ExaminationDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid ExaminationTypeId { get; set; }
        public string? ExaminationTypeName { get; set; }
        public Guid? RadiologistId { get; set; }
        public Guid? TechnicianId { get; set; }
        public Guid? EquipmentId { get; set; }
        public string? ClinicalIndication { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Paid { get; set; }
        public decimal Remaining { get; set; }
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    private sealed class ExaminationListItemDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid ExaminationTypeId { get; set; }
        public string? ExaminationTypeName { get; set; }
        public string? ClinicalIndication { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public decimal Price { get; set; }
        public decimal Paid { get; set; }
        public decimal Remaining { get; set; }
    }

    private sealed class ExaminationTypeListItemDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public decimal Price { get; set; }
    }

    private sealed class ItemListItemDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
    }

    private sealed class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    [Fact]
    public async Task AssignStaff_ValidData_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var radiologistId = Guid.NewGuid();
        var technicianId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var command = new
        {
            RadiologistId = radiologistId,
            TechnicianId = technicianId,
            EquipmentId = equipmentId
        };
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{examId}/staff", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AssignStaff_MissingRadiologistId_ReturnsBadRequest()
    {
        var examId = await CreateTestExaminationAsync();
        var technicianId = Guid.NewGuid();
        var command = new
        {
            TechnicianId = technicianId
        };
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{examId}/staff", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RecordPacsImages_ValidData_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var command = new
        {
            StudyInstanceUID = "1.2.3.4.5.6.7.8.9",
            AccessionNumber = $"ACC_{Guid.NewGuid():N}"
        };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/{examId}/pacs-images", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetExaminationTypeItems_ReturnsOk()
    {
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var response = await Client.GetAsync($"{BaseUrl}/examination-types/{examinationTypeId}/items");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ExaminationTypeItemDto>>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task AddExaminationTypeItem_ValidData_ReturnsOk()
    {
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var itemId = await GetOrCreateItemAsync();
        var command = new
        {
            ItemId = itemId,
            Quantity = 2,
            Notes = "Test item notes"
        };
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/examination-types/{examinationTypeId}/items", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCalendar_ReturnsOk()
    {
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(7);
        var url = $"{BaseUrl}/calendar?startDate={startDate:O}&endDate={endDate:O}";
        var response = await Client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<CalendarItemDto>>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAvailableSlots_ReturnsOk()
    {
        var date = DateTime.UtcNow.Date.AddDays(1);
        var equipmentId = Guid.NewGuid();
        var url = $"{BaseUrl}/available-slots?date={date:O}&equipmentId={equipmentId}";
        var response = await Client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<AvailableSlotDto>>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    private sealed class ExaminationTypeItemDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string? ItemName { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }

    private sealed class CalendarItemDto
    {
        public Guid Id { get; set; }
        public string? PatientName { get; set; }
        public string? ExaminationTypeName { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public string? Status { get; set; }
        public string? Modality { get; set; }
    }

    private sealed class AvailableSlotDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAvailable { get; set; }
    }
}
