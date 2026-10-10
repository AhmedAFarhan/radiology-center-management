using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class ExaminationsTests : TestBase
{
    private const string ExaminationsUrl = "api/examinations";
    private const string CatalogExaminationTypesUrl = "api/catalog/examination-types";
    private const string InventoryItemsUrl = "api/inventory/items";

    public ExaminationsTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_ValidData_ReturnsOk()
    {
        var patientId = await TestData.PatientAsync(Client);
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
        var response = await Client.PostAsJsonAsync(ExaminationsUrl, command);
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
        var response = await Client.PostAsJsonAsync(ExaminationsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_MissingExaminationTypeId_ReturnsBadRequest()
    {
        var patientId = await TestData.PatientAsync(Client);
        var command = new
        {
            PatientId = patientId,
            ClinicalIndication = "Test indication",
            Priority = "Routine"
        };
        var response = await Client.PostAsJsonAsync(ExaminationsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_MissingClinicalIndication_ReturnsBadRequest()
    {
        var patientId = await TestData.PatientAsync(Client);
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var command = new
        {
            PatientId = patientId,
            ExaminationTypeId = examinationTypeId,
            Priority = "Routine"
        };
        var response = await Client.PostAsJsonAsync(ExaminationsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_MissingPriority_ReturnsBadRequest()
    {
        var patientId = await TestData.PatientAsync(Client);
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var command = new
        {
            PatientId = patientId,
            ExaminationTypeId = examinationTypeId,
            ClinicalIndication = "Test indication"
        };
        var response = await Client.PostAsJsonAsync(ExaminationsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_NegativeDiscount_ReturnsBadRequest()
    {
        var patientId = await TestData.PatientAsync(Client);
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
        var response = await Client.PostAsJsonAsync(ExaminationsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var response = await Client.GetAsync($"{ExaminationsUrl}/{examId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(examId);
    }

    [Fact]
    public async Task GetById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{ExaminationsUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllPaged_ReturnsOk()
    {
        await CreateTestExaminationAsync();
        var request = new { PageNumber = 1, PageSize = 10 };
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/all", request);
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
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{examId}/schedule", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CheckIn_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var response = await Client.PostAsync($"{ExaminationsUrl}/{examId}/check-in", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Start_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync(status: "CheckedIn", paid: 100m);
        var response = await Client.PostAsync($"{ExaminationsUrl}/{examId}/start", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Complete_ReturnsOk()
    {
        var radiologistId = await TestData.StaffAsync(Client, "Radiologist");
        var technicianId = await TestData.StaffAsync(Client, "Technician");
        var equipmentId = await TestData.EquipmentAsync(Client);
        var examId = await CreateTestExaminationAsync(
            status: "CheckedIn",
            paid: 100m,
            radiologistId: radiologistId,
            technicianId: technicianId,
            equipmentId: equipmentId);

        await Client.PostAsync($"{ExaminationsUrl}/{examId}/start", null);
        var response = await Client.PostAsync($"{ExaminationsUrl}/{examId}/complete", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Cancel_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{examId}/cancel", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Cancel_WithReason_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync();
        var command = new { Reason = "Patient request" };
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{examId}/cancel", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponse = await Client.GetAsync($"{ExaminationsUrl}/{examId}");
        var body = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        body!.Data!.CancellationReason.Should().Be("Patient request");
    }

    [Fact]
    public async Task RecordPayment_ConcurrentFullPayments_CashIsRecordedOnce()
    {
        var id = await CreateTestExaminationAsync(paid: 0m);
        var initial = await Client.GetFromJsonAsync<ApiResponse<ExaminationDto>>($"{ExaminationsUrl}/{id}");
        var remaining = initial!.Data!.Remaining;
        var responses = await Task.WhenAll(Enumerable.Range(0, 2).Select(_ =>
            Client.PostAsJsonAsync($"{ExaminationsUrl}/{id}/payments", new { Amount = remaining })));
        responses.Count(r => r.StatusCode == HttpStatusCode.OK).Should().Be(1);
        responses.Count(r => r.StatusCode == HttpStatusCode.BadRequest).Should().Be(1);
        var saved = await Client.GetFromJsonAsync<ApiResponse<ExaminationDto>>($"{ExaminationsUrl}/{id}");
        saved!.Data!.Paid.Should().Be(remaining);
        saved.Data.Remaining.Should().Be(0m);
        var session = await Client.GetFromJsonAsync<System.Text.Json.JsonElement>("api/cash/sessions/my-open");
        var sessionId = session.GetProperty("data").GetProperty("id").GetGuid();
        var entries = await Client.GetFromJsonAsync<System.Text.Json.JsonElement>($"api/cash/sessions/{sessionId}/entries");
        var payments = entries.GetProperty("data").EnumerateArray().Where(e => e.GetProperty("referenceId").GetString() == id.ToString()).ToList();
        payments.Should().ContainSingle();
        payments[0].GetProperty("amount").GetDecimal().Should().Be(remaining);
    }

    [Theory]
    [InlineData("api/examinations/" + "00000000-0000-0000-0000-000000000001")]
    [InlineData("api/reports/00000000-0000-0000-0000-000000000001")]
    public async Task ClinicalEndpoints_WithoutToken_RejectAnonymousRequests(string path)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        using var anonymous = Factory.CreateClient();
        var response = await anonymous.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RecordPayment_ValidAmount_ReturnsOk()
    {
        var examId = await CreateTestExaminationAsync(paid: 0m);
        var command = new { Amount = 50m, Description = "Partial payment" };
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{examId}/payments", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RecordPayment_ExceedsRemaining_ReturnsBadRequest()
    {
        var examId = await CreateTestExaminationAsync(paid: 0m);
        var command = new { Amount = 999999m, Description = "Oversized payment" };
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{examId}/payments", command);
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
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{examId}/items", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateThenGetById_VerifyData()
    {
        var patientId = await TestData.PatientAsync(Client);
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
        var createResponse = await Client.PostAsJsonAsync(ExaminationsUrl, createCommand);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var createBody = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        var examId = createBody!.Data!.Id;

        var getResponse = await Client.GetAsync($"{ExaminationsUrl}/{examId}");
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
        var radiologistId = await TestData.StaffAsync(Client, "Radiologist");
        var technicianId = await TestData.StaffAsync(Client, "Technician");
        var equipmentId = await TestData.EquipmentAsync(Client);

        var examId = await CreateTestExaminationAsync(
            paid: 100m,
            radiologistId: radiologistId,
            technicianId: technicianId,
            equipmentId: equipmentId);

        var checkInResponse = await Client.PostAsync($"{ExaminationsUrl}/{examId}/check-in", null);
        checkInResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var startResponse = await Client.PostAsync($"{ExaminationsUrl}/{examId}/start", null);
        startResponse.StatusCode.Should().Be(HttpStatusCode.OK, await startResponse.Content.ReadAsStringAsync());

        var completeResponse = await Client.PostAsync($"{ExaminationsUrl}/{examId}/complete", null);
        completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var verifyResponse = await Client.GetAsync($"{ExaminationsUrl}/{examId}");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await verifyResponse.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        body!.Data!.Status.Should().Be("Completed");
        body.Data.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Cancel_NonexistentExamination_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{fakeId}/cancel", new { });
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
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{fakeId}/items", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RecordPayment_NonexistentExamination_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { Amount = 10m };
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{fakeId}/payments", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RecordPayment_ZeroAmount_ReturnsBadRequest()
    {
        var examId = await CreateTestExaminationAsync(paid: 0m);
        var command = new { Amount = 0m };
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{examId}/payments", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Start_WithOutstandingBalance_ReturnsError()
    {
        var examId = await CreateTestExaminationAsync(status: "CheckedIn");
        var response = await Client.PostAsync($"{ExaminationsUrl}/{examId}/start", null);
        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Book_ValidData_ReturnsOk()
    {
        var patientId = await TestData.PatientAsync(Client);
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var command = new
        {
            PatientId = patientId,
            ExaminationTypeId = examinationTypeId,
            ScheduledAt = DateTime.UtcNow.AddDays(1),
            Priority = "Routine",
            ClinicalIndication = "Book test"
        };
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/book", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Book_ConcurrentEquipmentReservation_OnlyOneSucceeds()
    {
        var equipment = await TestData.EquipmentAsync(Client);
        var type = await GetOrCreateExaminationTypeAsync();
        var patientOne = await TestData.PatientAsync(Client);
        var patientTwo = await TestData.PatientAsync(Client);
        var time = DateTime.UtcNow.AddDays(3);
        var responses = await Task.WhenAll(new[] { patientOne, patientTwo }.Select(patient =>
            Client.PostAsJsonAsync($"{ExaminationsUrl}/book", new
            {
                PatientId = patient, ExaminationTypeId = type, EquipmentId = equipment,
                ScheduledAt = time, ClinicalIndication = "Concurrent reservation", Priority = "Routine"
            })));
        responses.Count(r => r.StatusCode == HttpStatusCode.OK).Should().Be(1);
        responses.Count(r => r.StatusCode == HttpStatusCode.Conflict).Should().Be(1);
        var created = await responses.Single(r => r.IsSuccessStatusCode).Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        var persisted = await Client.GetFromJsonAsync<ApiResponse<ExaminationDto>>($"{ExaminationsUrl}/{created!.Data!.Id}");
        persisted!.Data!.ScheduledAt.Should().BeCloseTo(time, TimeSpan.FromSeconds(1));
        persisted.Data.AccessionNumber.Should().NotBeNullOrWhiteSpace().And.HaveLength(16);
    }

    [Fact]
    public async Task AvailableSlots_ZeroInterval_ReturnsBadRequest()
    {
        var equipment = await TestData.EquipmentAsync(Client);
        var date = DateTime.UtcNow.AddDays(1);
        var response = await Client.GetAsync($"{ExaminationsUrl}/available-slots?date={date:O}&equipmentId={equipment}&intervalMinutes=0");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Health_AfterProvisioning_ReturnsOk()
    {
        var response = await Client.GetAsync("health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
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
            PatientId = await TestData.PatientAsync(Client),
            ExaminationTypeId = examinationTypeId,
            ClinicalIndication = "Test indication",
            Priority = "Routine",
            Discount = 0m,
            IsDiscountPercentage = false,
            Paid = 0m,
            RadiologistId = radiologistId,
            TechnicianId = technicianId,
            EquipmentId = equipmentId,
            Status = status
        };
        var response = await Client.PostAsJsonAsync(ExaminationsUrl, command);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        if (paid is > 0)
        {
            var payment = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{body!.Data!.Id}/payments", new { Amount = body.Data.Remaining });
            payment.EnsureSuccessStatusCode();
            var verification = await Client.GetFromJsonAsync<ApiResponse<ExaminationDto>>($"{ExaminationsUrl}/{body.Data.Id}");
            verification!.Data!.Remaining.Should().Be(0, "payments must persist before the examination can start");
        }
        return body!.Data!.Id;
    }

    private async Task<Guid> GetOrCreateExaminationTypeAsync()
    {
        var createTypeCommand = new
        {
            Name = $"TestType_{Guid.NewGuid():N}",
            Code = $"TST_{Guid.NewGuid():N}",
            Modality = "XRay",
            BodyPart = "Chest",
            StandardDurationMinutes = 30,
            Price = 100m,
            RequiresPreparation = false,
            RequiresConsent = false,
            RequiresContrast = false
        };
        var createResponse = await Client.PostAsJsonAsync(CatalogExaminationTypesUrl, createTypeCommand);
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ExaminationTypeListItemDto>>();
        return created!.Data!.Id;
    }

    private async Task<Guid> GetOrCreateItemAsync()
    {
        var response = await Client.PostAsJsonAsync($"{InventoryItemsUrl}/all", new { PageNumber = 1, PageSize = 1 });
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
            Unit = "Piece",
            Category = "Consumable",
            UnitPrice = 10m
        };
        var createResponse = await Client.PostAsJsonAsync(InventoryItemsUrl, createCommand);
        createResponse.EnsureSuccessStatusCode();

        var allResponse = await Client.PostAsJsonAsync($"{InventoryItemsUrl}/all", new { PageNumber = 1, PageSize = 1 });
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
        public string? AccessionNumber { get; set; }
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
        var radiologistId = await TestData.StaffAsync(Client, "Radiologist");
        var technicianId = await TestData.StaffAsync(Client, "Technician");
        var equipmentId = await TestData.EquipmentAsync(Client);
        var command = new
        {
            RadiologistId = radiologistId,
            TechnicianId = technicianId,
            EquipmentId = equipmentId
        };
        var response = await Client.PutAsJsonAsync($"{ExaminationsUrl}/{examId}/staff", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AssignStaff_MissingRadiologistId_ReturnsBadRequest()
    {
        var examId = await CreateTestExaminationAsync();
        var technicianId = await TestData.StaffAsync(Client, "Technician");
        var command = new
        {
            TechnicianId = technicianId
        };
        var response = await Client.PutAsJsonAsync($"{ExaminationsUrl}/{examId}/staff", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RecordPacsImages_WrongAccession_IsRejected()
    {
        var examId = await CreateTestExaminationAsync();
        var command = new
        {
            StudyInstanceUID = "1.2.3.4.5.6.7.8.9",
            AccessionNumber = $"ACC_{Guid.NewGuid():N}"
        };
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/{examId}/pacs-images", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetExaminationTypeItems_ReturnsOk()
    {
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var response = await Client.GetAsync($"{ExaminationsUrl}/examination-types/{examinationTypeId}/items");
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
        var response = await Client.PostAsJsonAsync($"{ExaminationsUrl}/examination-types/{examinationTypeId}/items", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCalendar_ReturnsOk()
    {
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(7);
        var url = $"{ExaminationsUrl}/calendar?startDate={startDate:O}&endDate={endDate:O}";
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
        var equipmentId = await TestData.EquipmentAsync(Client);
        var url = $"{ExaminationsUrl}/available-slots?date={date:O}&equipmentId={equipmentId}";
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
