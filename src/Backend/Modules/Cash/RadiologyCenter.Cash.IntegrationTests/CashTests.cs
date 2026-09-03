using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class CashTests : TestBase
{
    private const string SessionsUrl = "api/cash/sessions";
    private const string HandoversUrl = "api/cash/handovers";

    public CashTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task OpenSession_ValidData_ReturnsOk()
    {
        var sessionId = await CreateTestSessionAsync();
        var response = await Client.GetAsync($"{SessionsUrl}/{sessionId}");
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.OpeningFloat.Should().Be(1000m);
        body.Data.Status.Should().Be("Open");
        await CloseTestSessionAsync(sessionId);
    }

    [Fact]
    public async Task OpenSession_MissingOpeningFloat_DefaultsToZero_ReturnsOk()
    {
        var command = new { Notes = "No float" };
        var response = await Client.PostAsJsonAsync(SessionsUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.OpeningFloat.Should().Be(0m);
        await CloseTestSessionAsync(result.Data.Id);
    }

    [Fact]
    public async Task OpenSession_NegativeOpeningFloat_ReturnsBadRequest()
    {
        var command = new { OpeningFloat = -500m };
        var response = await Client.PostAsJsonAsync(SessionsUrl, command);
        if (response.StatusCode != HttpStatusCode.BadRequest)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected BadRequest but got {response.StatusCode}: {body}");
        }
    }

    [Fact]
    public async Task OpenSession_OpenSessionAlreadyExists_ReturnsConflict()
    {
        await EnsureNoOpenSessionAsync();
        var command = new { OpeningFloat = 1000m };
        var first = await Client.PostAsJsonAsync(SessionsUrl, command);
        if (first.StatusCode != HttpStatusCode.OK)
        {
            var body = await first.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {first.StatusCode}: {body}");
        }

        var second = await Client.PostAsJsonAsync(SessionsUrl, command);
        if (second.StatusCode != HttpStatusCode.Conflict)
        {
            var body = await second.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected Conflict but got {second.StatusCode}: {body}");
        }
        var result = await second.Content.ReadFromJsonAsync<ApiResponse>();
        result!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task GetSessionById_Existing_ReturnsOk()
    {
        var sessionId = await CreateTestSessionAsync();
        var response = await Client.GetAsync($"{SessionsUrl}/{sessionId}");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(sessionId);
        await CloseTestSessionAsync(sessionId);
    }

    [Fact]
    public async Task GetSessionById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{SessionsUrl}/{fakeId}");
        if (response.StatusCode != HttpStatusCode.NotFound)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected NotFound but got {response.StatusCode}: {body}");
        }
    }

    [Fact]
    public async Task GetAllPaged_ReturnsOk()
    {
        var sessionId = await CreateTestSessionAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/all", request);
        var body = await response.Content.ReadAsStringAsync();
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        await CloseTestSessionAsync(sessionId);
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
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CashEntryDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Direction.Should().Be("In");
        result.Data.Amount.Should().Be(250m);
        await CloseTestSessionAsync(sessionId);
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
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CashEntryDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Direction.Should().Be("Out");
        result.Data.Amount.Should().Be(100m);
        await CloseTestSessionAsync(sessionId);
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
        if (response.StatusCode != HttpStatusCode.BadRequest)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected BadRequest but got {response.StatusCode}: {body}");
        }
        await CloseTestSessionAsync(sessionId);
    }

    [Fact]
    public async Task AddEntry_InvalidDirection_ReturnsBadRequest()
    {
        var sessionId = await CreateTestSessionAsync();
        var entry = new
        {
            Direction = "Invalid",
            Reason = "Payment",
            Amount = 100m,
            Description = "Invalid direction"
        };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry);
        if (response.StatusCode != HttpStatusCode.BadRequest)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected BadRequest but got {response.StatusCode}: {body}");
        }
        await CloseTestSessionAsync(sessionId);
    }

    [Fact]
    public async Task AddEntry_InvalidReason_ReturnsBadRequest()
    {
        var sessionId = await CreateTestSessionAsync();
        var entry = new
        {
            Direction = "In",
            Reason = "Invalid",
            Amount = 100m,
            Description = "Invalid reason"
        };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry);
        if (response.StatusCode != HttpStatusCode.BadRequest)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected BadRequest but got {response.StatusCode}: {body}");
        }
        await CloseTestSessionAsync(sessionId);
    }

    [Fact]
    public async Task AddEntry_ClosedSession_ReturnsError()
    {
        await EnsureNoOpenSessionAsync();
        var sessionId = await CreateTestSessionAsync();
        await CloseTestSessionAsync(sessionId);

        var entry = new
        {
            Direction = "In",
            Reason = "Payment",
            Amount = 50m
        };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry);
        if (response.StatusCode != HttpStatusCode.Conflict)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected Conflict but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        result!.Success.Should().BeFalse();
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
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CashHandoverDto>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.CashSessionId.Should().Be(sessionId);
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
        if (response.StatusCode != HttpStatusCode.BadRequest)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected BadRequest but got {response.StatusCode}: {body}");
        }
        await CloseTestSessionAsync(sessionId);
    }

    [Fact]
    public async Task CloseSession_AlreadyClosed_ReturnsError()
    {
        var sessionId = await CreateTestSessionAsync();
        await CloseTestSessionAsync(sessionId);

        var command = new { CountedTotal = 1000m };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/close", command);
        if (response.StatusCode != HttpStatusCode.Conflict)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected Conflict but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        result!.Success.Should().BeFalse();
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
        if (closeResponse.StatusCode != HttpStatusCode.OK)
        {
            var body = await closeResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {closeResponse.StatusCode}: {body}");
        }
    }

    [Fact]
    public async Task EndOfDay_NoSession_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{fakeId}/end-of-day", new { });
        if (response.StatusCode != HttpStatusCode.NotFound)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected NotFound but got {response.StatusCode}: {body}");
        }
    }

    [Fact]
    public async Task OpenSession_WithZeroOpeningFloat_ReturnsOk()
    {
        var sessionId = await CreateTestSessionAsync(0m);
        var response = await Client.GetAsync($"{SessionsUrl}/{sessionId}");
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.OpeningFloat.Should().Be(0m);
        await CloseTestSessionAsync(sessionId);
    }

    [Fact]
    public async Task OpenSession_WithNotes_VerifyNotesPersisted()
    {
        var command = new { OpeningFloat = 500m, Notes = "Night shift started" };
        var response = await Client.PostAsJsonAsync(SessionsUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Notes.Should().Be("Night shift started");
        await CloseTestSessionAsync(result.Data!.Id);
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
        if (closeResponse.StatusCode != HttpStatusCode.OK)
        {
            var body = await closeResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {closeResponse.StatusCode}: {body}");
        }

        var verifyResponse = await Client.GetAsync($"{SessionsUrl}/{sessionId}");
        var verifyBody = await verifyResponse.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        verifyBody!.Data!.Status.Should().Be("Closed");
        verifyBody.Data.ClosedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task GetHandovers_Paged_ReturnsOk()
    {
        await EnsureNoOpenSessionAsync();
        var sessionId = await CreateTestSessionAsync();
        await CloseTestSessionAsync(sessionId, 1000m);

        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{HandoversUrl}/all", request);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<CashHandoverDto>>>();
        result!.Success.Should().BeTrue();
        result.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetHandoverBySession_NonexistentSession_ReturnsOkWithNullData()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{HandoversUrl}/{fakeId}");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CashHandoverDto>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task ApproveHandover_NonexistentSession_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"{HandoversUrl}/{fakeId}/approve", new { });
        if (response.StatusCode != HttpStatusCode.NotFound)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected NotFound but got {response.StatusCode}: {body}");
        }
    }

    [Fact]
    public async Task GetMyOpenSession_WhenNoneOpen_ReturnsOkWithNullData()
    {
        await EnsureNoOpenSessionAsync();
        var response = await Client.GetAsync($"{SessionsUrl}/my-open");
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeNull();
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
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<CashEntryDto>>>();
        result!.Success.Should().BeTrue();
        result.Data!.Should().HaveCount(2);
        await CloseTestSessionAsync(sessionId);
    }

    private async Task EnsureNoOpenSessionAsync()
    {
        var response = await Client.GetAsync($"{SessionsUrl}/my-open");
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var session = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
            if (session?.Data?.Id is Guid openId)
            {
                await CloseTestSessionAsync(openId);
            }
        }
    }

    private async Task<Guid> CreateTestSessionAsync(decimal openingFloat = 1000m)
    {
        await EnsureNoOpenSessionAsync();

        var command = new { OpeningFloat = openingFloat, Notes = "Test session" };
        var response = await Client.PostAsJsonAsync(SessionsUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CashSessionDto>>();
        return result!.Data!.Id;
    }

    private async Task CloseTestSessionAsync(Guid sessionId, decimal countedTotal = 1000m)
    {
        var command = new { CountedTotal = countedTotal };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/close", command);
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return;
        }
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK or Conflict but got {response.StatusCode}: {body}");
        }
    }

    private async Task<Guid> AddTestEntryAsync(Guid sessionId, string direction, decimal amount)
    {
        var entry = new { Direction = direction, Reason = "Test", Amount = amount };
        var response = await Client.PostAsJsonAsync($"{SessionsUrl}/{sessionId}/entries", entry);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CashEntryDto>>();
        return result!.Data!.Id;
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

    private sealed class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
