using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class ReportsTests : TestBase
{
    private const string ReportsUrl = "api/reports";
    private const string TemplatesUrl = "api/reports/templates";
    private const string PatientsUrl = "api/patients";
    private const string ExaminationsUrl = "api/examinations";
    private const string ExaminationTypesUrl = "api/examination-types";

    public ReportsTests(CustomWebApplicationFactory factory) : base(factory) { }

    // ── Reports: Create Draft ──────────────────────────────────────────────

    [Fact]
    public async Task CreateDraft_ValidData_ReturnsOk()
    {
        var examinationId = await CreateTestExaminationAsync();
        var patientId = await CreateTestPatientAsync();
        var radiologistId = Guid.NewGuid();
        var command = new
        {
            ExaminationId = examinationId,
            PatientId = patientId,
            RadiologistId = radiologistId
        };
        var response = await Client.PostAsJsonAsync(ReportsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.ExaminationId.Should().Be(examinationId);
        body.Data.PatientId.Should().Be(patientId);
        body.Data.Status.Should().Be("Draft");
    }

    [Fact]
    public async Task CreateDraft_MissingExaminationId_ReturnsBadRequest()
    {
        var patientId = await CreateTestPatientAsync();
        var command = new
        {
            PatientId = patientId,
            RadiologistId = Guid.NewGuid()
        };
        var response = await Client.PostAsJsonAsync(ReportsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDraft_MissingPatientId_ReturnsBadRequest()
    {
        var examinationId = await CreateTestExaminationAsync();
        var command = new
        {
            ExaminationId = examinationId,
            RadiologistId = Guid.NewGuid()
        };
        var response = await Client.PostAsJsonAsync(ReportsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDraft_MissingRadiologistId_ReturnsBadRequest()
    {
        var examinationId = await CreateTestExaminationAsync();
        var patientId = await CreateTestPatientAsync();
        var command = new
        {
            ExaminationId = examinationId,
            PatientId = patientId
        };
        var response = await Client.PostAsJsonAsync(ReportsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── Reports: Get By Id ────────────────────────────────────────────────

    [Fact]
    public async Task GetById_ExistingReport_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        var response = await Client.GetAsync($"{ReportsUrl}/{reportId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(reportId);
    }

    [Fact]
    public async Task GetById_NonexistentReport_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{ReportsUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Reports: Get By Examination ────────────────────────────────────────

    [Fact]
    public async Task GetByExamination_ExistingExamination_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        var reportResponse = await Client.GetAsync($"{ReportsUrl}/{reportId}");
        var reportBody = await reportResponse.Content.ReadFromJsonAsync<ApiResponse<ReportDto>>();
        var examinationId = reportBody!.Data!.ExaminationId;

        var response = await Client.GetAsync($"{ReportsUrl}/by-examination/{examinationId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.ExaminationId.Should().Be(examinationId);
    }

    [Fact]
    public async Task GetByExamination_NonexistentExamination_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{ReportsUrl}/by-examination/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Reports: Get Versions ──────────────────────────────────────────────

    [Fact]
    public async Task GetVersions_ExistingReport_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        var response = await Client.GetAsync($"{ReportsUrl}/{reportId}/versions");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ReportVersionDto>>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeEmpty();
        body.Data!.First().VersionNumber.Should().Be(1);
    }

    [Fact]
    public async Task GetVersions_NonexistentReport_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{ReportsUrl}/{fakeId}/versions");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Reports: Get Paged ────────────────────────────────────────────────

    [Fact]
    public async Task GetAllPaged_ReturnsOk()
    {
        await CreateTestReportAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ReportListItemDto>>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.Items.Should().NotBeEmpty();
    }

    // ── Reports: Upsert Section ───────────────────────────────────────────

    [Fact]
    public async Task UpsertSection_ValidData_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        var command = new
        {
            SectionType = "ClinicalHistory",
            Title = "Clinical History",
            Body = "Patient presents with chest pain.",
            Position = 0,
            IsLocked = false
        };
        var response = await Client.PutAsJsonAsync($"{ReportsUrl}/{reportId}/sections", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task UpsertSection_NonexistentReport_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new
        {
            SectionType = "ClinicalHistory",
            Title = "Clinical History",
            Body = "Test body"
        };
        var response = await Client.PutAsJsonAsync($"{ReportsUrl}/{fakeId}/sections", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpsertSection_MissingSectionType_ReturnsBadRequest()
    {
        var reportId = await CreateTestReportAsync();
        var command = new
        {
            Title = "Clinical History",
            Body = "Test body"
        };
        var response = await Client.PutAsJsonAsync($"{ReportsUrl}/{reportId}/sections", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── Reports: Add Finding ──────────────────────────────────────────────

    [Fact]
    public async Task AddFinding_ValidData_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        var command = new
        {
            Region = "Right Lung",
            Description = "Opacity noted in right upper lobe.",
            Severity = "Moderate",
            Position = 0
        };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/findings", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportFindingDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.Region.Should().Be("Right Lung");
        body.Data.Description.Should().Be("Opacity noted in right upper lobe.");
        body.Data.Severity.Should().Be("Moderate");
    }

    [Fact]
    public async Task AddFinding_NonexistentReport_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new
        {
            Region = "Right Lung",
            Description = "Test finding",
            Severity = "Mild"
        };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{fakeId}/findings", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddFinding_MissingRegion_ReturnsBadRequest()
    {
        var reportId = await CreateTestReportAsync();
        var command = new
        {
            Description = "Test finding",
            Severity = "Mild"
        };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/findings", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── Reports: Update Finding ───────────────────────────────────────────

    [Fact]
    public async Task UpdateFinding_ValidData_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        var findingId = await AddTestFindingAsync(reportId);

        var command = new
        {
            Description = "Updated finding description.",
            Severity = "Severe"
        };
        var response = await Client.PutAsJsonAsync($"{ReportsUrl}/{reportId}/findings/{findingId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateFinding_NonexistentFinding_ReturnsNotFound()
    {
        var reportId = await CreateTestReportAsync();
        var fakeFindingId = Guid.NewGuid();
        var command = new
        {
            Description = "Updated description",
            Severity = "Severe"
        };
        var response = await Client.PutAsJsonAsync($"{ReportsUrl}/{reportId}/findings/{fakeFindingId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateFinding_NonexistentReport_ReturnsNotFound()
    {
        var fakeReportId = Guid.NewGuid();
        var fakeFindingId = Guid.NewGuid();
        var command = new
        {
            Description = "Updated description",
            Severity = "Severe"
        };
        var response = await Client.PutAsJsonAsync($"{ReportsUrl}/{fakeReportId}/findings/{fakeFindingId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Reports: Remove Finding ───────────────────────────────────────────

    [Fact]
    public async Task RemoveFinding_ExistingFinding_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        var findingId = await AddTestFindingAsync(reportId);

        var response = await Client.DeleteAsync($"{ReportsUrl}/{reportId}/findings/{findingId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveFinding_NonexistentFinding_ReturnsNotFound()
    {
        var reportId = await CreateTestReportAsync();
        var fakeFindingId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"{ReportsUrl}/{reportId}/findings/{fakeFindingId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveFinding_NonexistentReport_ReturnsNotFound()
    {
        var fakeReportId = Guid.NewGuid();
        var fakeFindingId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"{ReportsUrl}/{fakeReportId}/findings/{fakeFindingId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Reports: Apply Template ───────────────────────────────────────────

    [Fact]
    public async Task ApplyTemplate_ValidData_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        var templateId = await CreateTestTemplateAsync();

        var command = new { TemplateId = templateId };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/apply-template", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task ApplyTemplate_NonexistentReport_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var templateId = await CreateTestTemplateAsync();
        var command = new { TemplateId = templateId };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{fakeId}/apply-template", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ApplyTemplate_NonexistentTemplate_ReturnsNotFound()
    {
        var reportId = await CreateTestReportAsync();
        var fakeTemplateId = Guid.NewGuid();
        var command = new { TemplateId = fakeTemplateId };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/apply-template", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Reports: Finalize ─────────────────────────────────────────────────

    [Fact]
    public async Task Finalize_DraftReport_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/finalize", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Status.Should().Be("Finalized");
        body.Data.FinalizedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Finalize_NonexistentReport_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{fakeId}/finalize", new { });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Finalize_AlreadyFinalized_ReturnsError()
    {
        var reportId = await CreateTestReportAsync();
        await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/finalize", new { });

        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/finalize", new { });
        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    // ── Reports: Amend ────────────────────────────────────────────────────

    [Fact]
    public async Task Amend_FinalizedReport_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/finalize", new { });

        var command = new { Reason = "Correction needed for findings" };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/amend", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Amend_NonexistentReport_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { Reason = "Missing report" };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{fakeId}/amend", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Amend_DraftReport_ReturnsError()
    {
        var reportId = await CreateTestReportAsync();
        var command = new { Reason = "Cannot amend draft" };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/amend", command);
        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    // ── Reports: Cancel ───────────────────────────────────────────────────

    [Fact]
    public async Task Cancel_DraftReport_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        var command = new { Reason = "Patient requested cancellation" };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/cancel", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Cancel_WithoutReason_ReturnsOk()
    {
        var reportId = await CreateTestReportAsync();
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/cancel", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Cancel_NonexistentReport_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { Reason = "Missing report" };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{fakeId}/cancel", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Report Templates: Create ──────────────────────────────────────────

    [Fact]
    public async Task CreateTemplate_ValidData_ReturnsOk()
    {
        var command = new
        {
            Name = $"Template_{Guid.NewGuid():N}",
            Modality = "X-Ray",
            BodyPart = "Chest",
            Description = "Standard chest X-ray report template"
        };
        var response = await Client.PostAsJsonAsync(TemplatesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportTemplateDto>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.Name.Should().Be(command.Name);
        body.Data.Modality.Should().Be("X-Ray");
        body.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateTemplate_WithSections_ReturnsOk()
    {
        var command = new
        {
            Name = $"TemplateWithSections_{Guid.NewGuid():N}",
            Modality = "MRI",
            Sections = new[]
            {
                new
                {
                    SectionType = "ClinicalHistory",
                    Title = "Clinical History",
                    Body = "Default clinical history text",
                    Position = 0,
                    IsLocked = true
                },
                new
                {
                    SectionType = "Findings",
                    Title = "Findings",
                    Body = "Default findings text",
                    Position = 1,
                    IsLocked = false
                }
            }
        };
        var response = await Client.PostAsJsonAsync(TemplatesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportTemplateDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Sections.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateTemplate_MissingName_ReturnsBadRequest()
    {
        var command = new
        {
            Modality = "X-Ray"
        };
        var response = await Client.PostAsJsonAsync(TemplatesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTemplate_MissingModality_ReturnsBadRequest()
    {
        var command = new
        {
            Name = $"Template_{Guid.NewGuid():N}"
        };
        var response = await Client.PostAsJsonAsync(TemplatesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ── Report Templates: Get By Id ───────────────────────────────────────

    [Fact]
    public async Task GetTemplateById_Existing_ReturnsOk()
    {
        var templateId = await CreateTestTemplateAsync();
        var response = await Client.GetAsync($"{TemplatesUrl}/{templateId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportTemplateDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(templateId);
    }

    [Fact]
    public async Task GetTemplateById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{TemplatesUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Report Templates: Get Paged ───────────────────────────────────────

    [Fact]
    public async Task GetTemplatesPaged_ReturnsOk()
    {
        await CreateTestTemplateAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{TemplatesUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ReportTemplateDto>>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.Items.Should().NotBeEmpty();
    }

    // ── Report Templates: Update ──────────────────────────────────────────

    [Fact]
    public async Task UpdateTemplate_ValidData_ReturnsOk()
    {
        var templateId = await CreateTestTemplateAsync();
        var command = new
        {
            Name = "Updated Template Name",
            Modality = "CT",
            BodyPart = "Abdomen",
            Description = "Updated description"
        };
        var response = await Client.PutAsJsonAsync($"{TemplatesUrl}/{templateId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportTemplateDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Name.Should().Be("Updated Template Name");
        body.Data.Modality.Should().Be("CT");
    }

    [Fact]
    public async Task UpdateTemplate_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new
        {
            Name = "Updated Name",
            Modality = "X-Ray"
        };
        var response = await Client.PutAsJsonAsync($"{TemplatesUrl}/{fakeId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Report Templates: Add Section ─────────────────────────────────────

    [Fact]
    public async Task AddTemplateSection_ValidData_ReturnsOk()
    {
        var templateId = await CreateTestTemplateAsync();
        var command = new
        {
            Section = new
            {
                SectionType = "Impression",
                Title = "Impression",
                Body = "Default impression text",
                Position = 0,
                IsLocked = false
            }
        };
        var response = await Client.PostAsJsonAsync($"{TemplatesUrl}/{templateId}/sections", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportTemplateDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Sections.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddTemplateSection_NonexistentTemplate_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new
        {
            Section = new
            {
                SectionType = "Impression",
                Title = "Impression",
                Body = "Test"
            }
        };
        var response = await Client.PostAsJsonAsync($"{TemplatesUrl}/{fakeId}/sections", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Report Templates: Remove Section ──────────────────────────────────

    [Fact]
    public async Task RemoveTemplateSection_ValidSection_ReturnsOk()
    {
        var templateId = await CreateTestTemplateAsync();
        var templateResponse = await Client.GetAsync($"{TemplatesUrl}/{templateId}");
        var templateBody = await templateResponse.Content.ReadFromJsonAsync<ApiResponse<ReportTemplateDto>>();
        var sectionId = templateBody!.Data!.Sections!.First().Id;

        var response = await Client.DeleteAsync($"{TemplatesUrl}/{templateId}/sections/{sectionId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportTemplateDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Sections.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveTemplateSection_NonexistentSection_ReturnsNotFound()
    {
        var templateId = await CreateTestTemplateAsync();
        var fakeSectionId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"{TemplatesUrl}/{templateId}/sections/{fakeSectionId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveTemplateSection_NonexistentTemplate_ReturnsNotFound()
    {
        var fakeTemplateId = Guid.NewGuid();
        var fakeSectionId = Guid.NewGuid();

        var response = await Client.DeleteAsync($"{TemplatesUrl}/{fakeTemplateId}/sections/{fakeSectionId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Report Templates: Activate ────────────────────────────────────────

    [Fact]
    public async Task ActivateTemplate_ReturnsOk()
    {
        var templateId = await CreateTestTemplateAsync();
        await DeactivateTestTemplateAsync(templateId);

        var response = await Client.PostAsJsonAsync($"{TemplatesUrl}/{templateId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ActivateTemplate_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"{TemplatesUrl}/{fakeId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Report Templates: Deactivate ──────────────────────────────────────

    [Fact]
    public async Task DeactivateTemplate_ReturnsOk()
    {
        var templateId = await CreateTestTemplateAsync();
        var response = await Client.PostAsJsonAsync($"{TemplatesUrl}/{templateId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeactivateTemplate_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.PostAsJsonAsync($"{TemplatesUrl}/{fakeId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Report Templates: Delete ──────────────────────────────────────────

    [Fact]
    public async Task DeleteTemplate_ReturnsOk()
    {
        var templateId = await CreateTestTemplateAsync();
        var response = await Client.DeleteAsync($"{TemplatesUrl}/{templateId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await Client.GetAsync($"{TemplatesUrl}/{templateId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTemplate_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{TemplatesUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private async Task<Guid> CreateTestReportAsync()
    {
        var examinationId = await CreateTestExaminationAsync();
        var patientId = await CreateTestPatientAsync();
        var command = new
        {
            ExaminationId = examinationId,
            PatientId = patientId,
            RadiologistId = Guid.NewGuid()
        };
        var response = await Client.PostAsJsonAsync(ReportsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportDto>>();
        return body!.Data!.Id;
    }

    private async Task<Guid> AddTestFindingAsync(Guid reportId)
    {
        var command = new
        {
            Region = "Right Lung",
            Description = "Opacity noted.",
            Severity = "Moderate",
            Position = 0
        };
        var response = await Client.PostAsJsonAsync($"{ReportsUrl}/{reportId}/findings", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportFindingDto>>();
        return body!.Data!.Id;
    }

    private async Task<Guid> CreateTestTemplateAsync()
    {
        var command = new
        {
            Name = $"Template_{Guid.NewGuid():N}",
            Modality = "X-Ray",
            BodyPart = "Chest",
            Description = "Test template"
        };
        var response = await Client.PostAsJsonAsync(TemplatesUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReportTemplateDto>>();
        return body!.Data!.Id;
    }

    private async Task DeactivateTestTemplateAsync(Guid templateId)
    {
        var response = await Client.PostAsJsonAsync($"{TemplatesUrl}/{templateId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> CreateTestPatientAsync()
    {
        var command = new
        {
            FullName = "Ahmed Mohamed Ali",
            Gender = "Male",
            PhoneNumber = $"010{Random.Shared.Next(10000000, 99999999)}",
            DateOfBirth = new DateTime(1990, 5, 15)
        };
        var response = await Client.PostAsJsonAsync(PatientsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PatientDto>>();
        return body!.Data!.Id;
    }

    private async Task<Guid> CreateTestExaminationAsync()
    {
        var examinationTypeId = await GetOrCreateExaminationTypeAsync();
        var patientId = await CreateTestPatientAsync();
        var command = new
        {
            PatientId = patientId,
            ExaminationTypeId = examinationTypeId,
            ClinicalIndication = "Test indication",
            Priority = "Routine",
            Discount = 0m,
            IsDiscountPercentage = false,
            Paid = 0m
        };
        var response = await Client.PostAsJsonAsync(ExaminationsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationDto>>();
        return body!.Data!.Id;
    }

    private async Task<Guid> GetOrCreateExaminationTypeAsync()
    {
        var response = await Client.PostAsJsonAsync($"{ExaminationTypesUrl}/all", new { PageNumber = 1, PageSize = 1 });
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
        var createResponse = await Client.PostAsJsonAsync(ExaminationTypesUrl, createTypeCommand);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var createBody = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ExaminationTypeListItemDto>>();
        return createBody!.Data!.Id;
    }

    // ── DTOs ──────────────────────────────────────────────────────────────

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

    private sealed class ReportDto
    {
        public Guid Id { get; set; }
        public Guid ExaminationId { get; set; }
        public Guid PatientId { get; set; }
        public Guid RadiologistId { get; set; }
        public string? Status { get; set; }
        public string? StatusKey { get; set; }
        public int CurrentVersionNumber { get; set; }
        public DateTime? FinalizedAt { get; set; }
        public string? CancelReason { get; set; }
        public ReportVersionDto? CurrentVersion { get; set; }
        public string? PatientName { get; set; }
        public string? RadiologistName { get; set; }
        public string? ExaminationTypeName { get; set; }
    }

    private sealed class ReportListItemDto
    {
        public Guid Id { get; set; }
        public Guid ExaminationId { get; set; }
        public Guid PatientId { get; set; }
        public Guid RadiologistId { get; set; }
        public string? Status { get; set; }
        public string? StatusKey { get; set; }
        public int CurrentVersionNumber { get; set; }
        public DateTime? FinalizedAt { get; set; }
        public string? CancelReason { get; set; }
        public string? PatientName { get; set; }
        public string? RadiologistName { get; set; }
        public string? ExaminationTypeName { get; set; }
    }

    private sealed class ReportVersionDto
    {
        public Guid Id { get; set; }
        public int VersionNumber { get; set; }
        public string? AmendmentReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ReportSectionDto>? Sections { get; set; }
        public List<ReportFindingDto>? Findings { get; set; }
    }

    private sealed class ReportSectionDto
    {
        public Guid Id { get; set; }
        public string? SectionType { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public int Position { get; set; }
        public bool IsLocked { get; set; }
    }

    private sealed class ReportFindingDto
    {
        public Guid Id { get; set; }
        public string? Region { get; set; }
        public string? Description { get; set; }
        public string? Severity { get; set; }
        public int Position { get; set; }
    }

    private sealed class ReportTemplateDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Modality { get; set; }
        public string? BodyPart { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public int UseCount { get; set; }
        public List<ReportTemplateSectionDto>? Sections { get; set; }
    }

    private sealed class ReportTemplateSectionDto
    {
        public Guid Id { get; set; }
        public string? SectionType { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public int Position { get; set; }
        public bool IsLocked { get; set; }
    }

    private sealed class PatientDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
    }

    private sealed class ExaminationDto
    {
        public Guid Id { get; set; }
    }

    private sealed class ExaminationTypeListItemDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }

    private sealed class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
