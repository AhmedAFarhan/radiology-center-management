using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class NotificationTests : TestBase
{
    private const string TemplatesUrl = "api/notifications/templates";
    private const string MessagesUrl = "api/notifications";

    public NotificationTests(CustomWebApplicationFactory factory) : base(factory) { }

    // ───────────────────────── Templates: Create ─────────────────────────

    [Fact]
    public async Task CreateTemplate_ValidData_ReturnsOk()
    {
        var command = new
        {
            Code = $"TPL_{Guid.NewGuid():N}",
            Name = $"Template {Guid.NewGuid():N}",
            Subject = "Appointment Reminder",
            Body = "Your appointment is on {{Date}}"
        };

        var response = await Client.PostAsJsonAsync(TemplatesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<NotificationTemplateDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.Code.Should().Be(command.Code);
        body.Data.Name.Should().Be(command.Name);
        body.Data.Subject.Should().Be(command.Subject);
        body.Data.Body.Should().Be(command.Body);
        body.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateTemplate_MissingCode_ReturnsBadRequest()
    {
        var command = new
        {
            Code = "",
            Name = $"Template {Guid.NewGuid():N}",
            Subject = "Subject",
            Body = "Body"
        };

        var response = await Client.PostAsJsonAsync(TemplatesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTemplate_MissingName_ReturnsBadRequest()
    {
        var command = new
        {
            Code = $"TPL_{Guid.NewGuid():N}",
            Name = "",
            Subject = "Subject",
            Body = "Body"
        };

        var response = await Client.PostAsJsonAsync(TemplatesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTemplate_MissingSubject_ReturnsBadRequest()
    {
        var command = new
        {
            Code = $"TPL_{Guid.NewGuid():N}",
            Name = $"Template {Guid.NewGuid():N}",
            Subject = "",
            Body = "Body"
        };

        var response = await Client.PostAsJsonAsync(TemplatesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTemplate_MissingBody_ReturnsBadRequest()
    {
        var command = new
        {
            Code = $"TPL_{Guid.NewGuid():N}",
            Name = $"Template {Guid.NewGuid():N}",
            Subject = "Subject",
            Body = ""
        };

        var response = await Client.PostAsJsonAsync(TemplatesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ─────────────────── Templates: Get By Id ────────────────────────────

    [Fact]
    public async Task GetTemplateById_Existing_ReturnsOk()
    {
        var id = await CreateTestTemplateAsync();
        var response = await Client.GetAsync($"{TemplatesUrl}/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<NotificationTemplateDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetTemplateById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{TemplatesUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ───────────────── Templates: Get Paged ──────────────────────────────

    [Fact]
    public async Task GetTemplatesPaged_ReturnsOk()
    {
        await CreateTestTemplateAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{TemplatesUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<NotificationTemplateDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    // ──────────────────── Templates: Update ──────────────────────────────

    [Fact]
    public async Task UpdateTemplate_ValidData_ReturnsOk()
    {
        var id = await CreateTestTemplateAsync();
        var command = new
        {
            Id = id,
            Code = $"UPD_{Guid.NewGuid():N}",
            Name = $"Updated Template {Guid.NewGuid():N}",
            Subject = "Updated Subject",
            Body = "Updated Body"
        };

        var response = await Client.PutAsJsonAsync($"{TemplatesUrl}/{id}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateTemplate_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new
        {
            Id = fakeId,
            Code = $"GHOST_{Guid.NewGuid():N}",
            Name = "Ghost Template",
            Subject = "Ghost Subject",
            Body = "Ghost Body"
        };

        var response = await Client.PutAsJsonAsync($"{TemplatesUrl}/{fakeId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ─────────────── Templates: Activate / Deactivate ────────────────────

    [Fact]
    public async Task ActivateTemplate_ReturnsOk()
    {
        var id = await CreateTestTemplateAsync();
        var response = await Client.PostAsJsonAsync($"{TemplatesUrl}/{id}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeactivateTemplate_ReturnsOk()
    {
        var id = await CreateTestTemplateAsync();
        var response = await Client.PostAsJsonAsync($"{TemplatesUrl}/{id}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ──────────────── Templates: Delete ──────────────────────────────────

    [Fact]
    public async Task DeleteTemplate_ValidData_ReturnsOk()
    {
        var id = await CreateTestTemplateAsync();
        var response = await Client.DeleteAsync($"{TemplatesUrl}/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteTemplate_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{TemplatesUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ───────────────── Messages: Send ────────────────────────────────────

    [Fact]
    public async Task SendNotification_WithTemplateCode_ReturnsOk()
    {
        var code = await CreateTestTemplateAsync();
        var command = new
        {
            Recipient = "patient@example.com",
            Channel = "Email",
            TemplateCode = code,
            Placeholders = new Dictionary<string, string> { { "Date", "2026-09-15" } }
        };

        var response = await Client.PostAsJsonAsync($"{MessagesUrl}/send", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendNotification_WithInlineSubjectAndBody_ReturnsOk()
    {
        var command = new
        {
            Recipient = "+1234567890",
            Channel = "Sms",
            Subject = "Inline Subject",
            Body = "Inline body text"
        };

        var response = await Client.PostAsJsonAsync($"{MessagesUrl}/send", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendNotification_MissingRecipient_ReturnsBadRequest()
    {
        var command = new
        {
            Recipient = "",
            Channel = "Email",
            TemplateCode = "some-code",
            Subject = "Subject",
            Body = "Body"
        };

        var response = await Client.PostAsJsonAsync($"{MessagesUrl}/send", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SendNotification_MissingChannel_ReturnsBadRequest()
    {
        var command = new
        {
            Recipient = "user@example.com",
            Channel = "",
            TemplateCode = "some-code",
            Subject = "Subject",
            Body = "Body"
        };

        var response = await Client.PostAsJsonAsync($"{MessagesUrl}/send", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SendNotification_MissingTemplateAndBody_ReturnsBadRequest()
    {
        var command = new
        {
            Recipient = "user@example.com",
            Channel = "Email",
            TemplateCode = (string?)null,
            Subject = "Only subject, no body or template",
            Body = (string?)null
        };

        var response = await Client.PostAsJsonAsync($"{MessagesUrl}/send", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ───────────────── Messages: Preview ─────────────────────────────────

    [Fact]
    public async Task PreviewNotification_ReturnsOk()
    {
        var command = new
        {
            Recipient = "preview@example.com",
            Channel = "Email",
            Subject = "Preview Subject",
            Body = "Preview body with {{placeholder}}"
        };

        var response = await Client.PostAsJsonAsync($"{MessagesUrl}/preview", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<NotificationMessageDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    // ───────────────── Messages: Get Paged ───────────────────────────────

    [Fact]
    public async Task GetMessagesPaged_ReturnsOk()
    {
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{MessagesUrl}/messages/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<NotificationMessageDto>>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    // ──────────────────── Helpers ────────────────────────────────────────

    private async Task<string> CreateTestTemplateAsync()
    {
        var code = $"TPL_{Guid.NewGuid():N}";
        var command = new
        {
            Code = code,
            Name = $"Template {Guid.NewGuid():N}",
            Subject = "Test Subject",
            Body = "Test Body"
        };

        var response = await Client.PostAsJsonAsync(TemplatesUrl, command);
        response.EnsureSuccessStatusCode();
        return code;
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

    private sealed class NotificationTemplateDto
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class NotificationMessageDto
    {
        public Guid Id { get; set; }
        public string? Recipient { get; set; }
        public string? Channel { get; set; }
        public string? Status { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? TemplateCode { get; set; }
        public string? ReferenceId { get; set; }
        public int Attempts { get; set; }
        public DateTime? SentAtUtc { get; set; }
        public string? FailureReason { get; set; }
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
