using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class CashTests : TestBase
{
    private const string SessionsUrl = "api/cash/sessions";

    public CashTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task OpenSession_ValidData_ReturnsOk()
    {
        var command = new { OpeningFloat = 1000m, Notes = "Morning shift" };
        var response = await Client.PostAsJsonAsync(SessionsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.OpeningFloat.Should().Be(1000m);
        body.Data.Status.Should().Be("Open");
    }

    [Fact]
    public async Task OpenSession_MissingOpeningFloat_ReturnsBadRequest()
    {
        var command = new { Notes = "No float" };
        var response = await Client.PostAsJsonAsync(SessionsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task OpenSession_NegativeOpeningFloat_ReturnsBadRequest()
    {
        var command = new { OpeningFloat = -500m };
        var response = await Client.PostAsJsonAsync(SessionsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task OpenSession_OpenSessionAlreadyExists_ReturnsConflict()
    {
        var command = new { OpeningFloat = 1000m };
        var first = await Client.PostAsJsonAsync(SessionsUrl, command);
        first.StatusCode.Should().Be(HttpStatusCode.OK);

        var second = await Client.PostAsJsonAsync(SessionsUrl, command);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await second.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task GetSessionById_Existing_ReturnsOk()
    {
        var sessionId = await CreateTestSessionAsync();
        var response = await Client.GetAsync($"{SessionsUrl}/{sessionId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(sessionId);
    }

    [Fact]
    public async Task GetSessionById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{SessionsUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllPaged_ReturnsOk()
    {
        await CreateTestSessionAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<CashSessionDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddEntry_ValidInEntry_ReturnsOk()
    {
        var sessionId = await CreateTestSessionAsync();
        var entry = new
        {
            Direction = "In",
            Reason = "Payment",
            Amount = 250m,
            Description = "Consultation payment"
        };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CashEntryDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Direction.Should().Be("In");
        body.Data.Amount.Should().Be(250m);
    }

    [Fact]
    public async Task AddEntry_ValidOutEntry_ReturnsOk()
    {
        var sessionId = await CreateTestSessionAsync();
        var entry = new
        {
            Direction = "Out",
            Reason = "Payout",
            Amount = 100m,
            Description = "Supplies refund"
        };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CashEntryDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Direction.Should().Be("Out");
        body.Data.Amount.Should().Be(100m);
    }

    [Fact]
    public async Task AddEntry_MissingAmount_ReturnsBadRequest()
    {
        var sessionId = await CreateTestSessionAsync();
        var entry = new
        {
            Direction = "In",
            Reason = "Payment",
            Description = "Missing amount"
        };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddEntry_MissingDirection_ReturnsBadRequest()
    {
        var sessionId = await CreateTestSessionAsync();
        var entry = new
        {
            Reason = "Payment",
            Amount = 100m,
            Description = "Missing direction"
        };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddEntry_MissingReason_ReturnsBadRequest()
    {
        var sessionId = await CreateTestSessionAsync();
        var entry = new
        {
            Direction = "In",
            Amount = 100m,
            Description = "Missing reason"
        };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddEntry_ClosedSession_ReturnsError()
    {
        var sessionId = await CreateTestSessionAsync();
        await CloseTestSessionAsync(sessionId);

        var entry = new
        {
            Direction = "In",
            Reason = "Payment",
            Amount = 50m
        };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task CloseSession_ValidData_ReturnsOk()
    {
        var sessionId = await CreateTestSessionAsync();
        var command = new
        {
            CountedTotal = 1000m,
            Notes = "End of shift"
        };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/close", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CashHandoverDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.CashSessionId.Should().Be(sessionId);
    }

    [Fact]
    public async Task CloseSession_NegativeCountedTotal_ReturnsBadRequest()
    {
        var sessionId = await CreateTestSessionAsync();
        var command = new
        {
            CountedTotal = -100m
        };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/close", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CloseSession_AlreadyClosed_ReturnsError()
    {
        var sessionId = await CreateTestSessionAsync();
        await CloseTestSessionAsync(sessionId);

        var command = new { CountedTotal = 1000m };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/close", command);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task AddEntry_ThenClose_VerifyBalance()
    {
        var sessionId = await CreateTestSessionAsync(500m);

        var inEntry = new { Direction = "In", Reason = "Payment", Amount = 300m };
        await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", inEntry);

        var outEntry = new { Direction = "Out", Reason = "Payout", Amount = 50m };
        await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", outEntry);

        var sessionResponse = await Client.GetAsync($"{SessionsUrl}/{sessionId}");
        var sessionBody = await sessionResponse.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        sessionBody!.Data!.EntryCount.Should().Be(2);

        var command = new { CountedTotal = 750m };
        var closeResponse = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/close", command);
        closeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task EndOfDay_NoSession_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{fakeId}/end-of-day", new { });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task OpenSession_WithZeroOpeningFloat_ReturnsOk()
    {
        var command = new { OpeningFloat = 0m, Notes = "Zero start" };
        var response = await Client.PostAsJsonAsync(SessionsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.OpeningFloat.Should().Be(0m);
    }

    [Fact]
    public async Task OpenSession_WithNotes_VerifyNotesPersisted()
    {
        var command = new { OpeningFloat = 500m, Notes = "Night shift started" };
        var response = await Client.PostAsJsonAsync(SessionsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Notes.Should().Be("Night shift started");
    }

    [Fact]
    public async Task OpenAddEntryClose_VerifySessionSummary()
    {
        var sessionId = await CreateTestSessionAsync(200m);

        var inEntry1 = new { Direction = "In", Reason = "Payment", Amount = 150m };
        await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", inEntry1);

        var inEntry2 = new { Direction = "In", Reason = "Deposit", Amount = 100m };
        await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", inEntry2);

        var outEntry = new { Direction = "Out", Reason = "Refund", Amount = 50m };
        await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", outEntry);

        var sessionResponse = await Client.GetAsync($"{SessionsUrl}/{sessionId}");
        var sessionBody = await sessionResponse.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        sessionBody!.Data!.EntryCount.Should().Be(3);
        sessionBody.Data.OpeningFloat.Should().Be(200m);
        sessionBody.Data.Balance.Should().Be(400m);

        var command = new { CountedTotal = 400m, Notes = "Balanced" };
        var closeResponse = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/close", command);
        closeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var verifyResponse = await Client.GetAsync($"{SessionsUrl}/{sessionId}");
        var verifyBody = await verifyResponse.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        verifyBody!.Data!.Status.Should().Be("Closed");
        verifyBody.Data.ClosedAt.Should().NotBeNull();
    }

    private async Task<Guid> CreateTestSessionAsync(decimal openingFloat = 1000m)
    {
        var command = new { OpeningFloat = openingFloat, Notes = "Test session" };
        var response = await Client.PostAsJsonAsync(SessionsUrl, command);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        return body!.Data!.Id;
    }

    private async Task CloseTestSessionAsync(Guid sessionId, decimal countedTotal = 1000m)
    {
        var command = new { CountedTotal = countedTotal };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/close", command);
        response.EnsureSuccessStatusCode();
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

    private sealed class CashSessionDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public Guid? WorkShiftId { get; set; }
        public string? Status { get; set; }
        public decimal OpeningFloat { get; set; }
        public decimal Balance { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string? Notes { get; set; }
        public int EntryCount { get; set; }
        public string? StatusKey { get; set; }
    }

    private sealed class CashEntryDto
    {
        public Guid Id { get; set; }
        public Guid CashSessionId { get; set; }
        public string? Direction { get; set; }
        public string? Reason { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? ReferenceId { get; set; }
        public DateTime OccurredAt { get; set; }
        public string? DirectionKey { get; set; }
        public string? ReasonKey { get; set; }
    }

    private sealed class CashHandoverDto
    {
        public Guid Id { get; set; }
        public Guid CashSessionId { get; set; }
        public decimal ExpectedTotal { get; set; }
        public decimal CountedTotal { get; set; }
        public decimal OverShortAmount { get; set; }
        public DateTime ClosedAt { get; set; }
        public Guid ClosedByUserId { get; set; }
        public string? ClosedByName { get; set; }
        public Guid? ReceivingCashSessionId { get; set; }
        public string? Notes { get; set; }
    }

    [Fact]
    public async Task GetHandovers_Paged_ReturnsOk()
    {
        var sessionId = await CreateTestSessionAsync();
        await CloseTestSessionAsync(sessionId, 1000m);

        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync("api/cash/handovers/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<CashHandoverDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetHandoverBySession_NonexistentSession_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"api/cash/handovers/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ApproveHandover_NonexistentSession_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"api/cash/handovers/{fakeId}/approve", new { });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMyOpenSession_WhenNoneOpen_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"{SessionsUrl}/my-open");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCashEntries_AfterAddingEntries_ReturnsOk()
    {
        var sessionId = await CreateTestSessionAsync();
        var entry1 = new { Direction = "In", Reason = "Payment", Amount = 100m, Description = "First payment" };
        await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry1);
        var entry2 = new { Direction = "Out", Reason = "Payout", Amount = 50m, Description = "Supplies" };
        await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry2);

        var response = await Client.GetAsync($"{SessionsUrl}/{sessionId}/entries");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<CashEntryDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Should().HaveCount(2);
    }

    private async Task<Guid> AddTestEntryAsync(Guid sessionId, string direction, decimal amount)
    {
        var entry = new { Direction = direction, Reason = "Test", Amount = amount };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CashEntryDto>>();
        return body!.Data!.Id;
    }

    private sealed class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
