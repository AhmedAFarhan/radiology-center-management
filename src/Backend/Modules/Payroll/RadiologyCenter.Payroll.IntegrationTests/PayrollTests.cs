using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class PayrollTests : TestBase
{
    private const string PayRunsBaseUrl = "api/payroll/payruns";
    private const string SalariesBaseUrl = "api/payroll/salaries";
    private const string SalaryComponentsBaseUrl = "api/payroll/salary-components";
    private const string ExaminationFeesBaseUrl = "api/payroll/examination-fees";
    private const string ReferralFeesBaseUrl = "api/payroll/referral-fees";
    private const string AllowancesBaseUrl = "api/payroll/allowances";
    private const string StaffBaseUrl = "api/resources/staff";
    private const string ReferralDoctorsBaseUrl = "api/resources/referral-doctors";
    private const string ExaminationTypesBaseUrl = "api/catalog/examination-types";

    public PayrollTests(CustomWebApplicationFactory factory) : base(factory) { }

    #region PayRuns

    [Fact]
    public async Task CreatePayRun_ValidData_ReturnsOk()
    {
        var command = new
        {
            RunFrom = DateTime.UtcNow.Date.AddDays(-30),
            RunTo = DateTime.UtcNow.Date,
            Notes = "Test pay run"
        };
        var response = await Client.PostAsJsonAsync(PayRunsBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreatePayRun_MissingRunFrom_ReturnsBadRequest()
    {
        var command = new
        {
            RunFrom = default(DateTime),
            RunTo = DateTime.UtcNow.Date,
            Notes = "Missing RunFrom"
        };
        var response = await Client.PostAsJsonAsync(PayRunsBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePayRun_MissingRunTo_ReturnsBadRequest()
    {
        var command = new
        {
            RunFrom = DateTime.UtcNow.Date.AddDays(-30),
            RunTo = default(DateTime),
            Notes = "Missing RunTo"
        };
        var response = await Client.PostAsJsonAsync(PayRunsBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePayRun_RunToBeforeRunFrom_ReturnsBadRequest()
    {
        var command = new
        {
            RunFrom = DateTime.UtcNow.Date,
            RunTo = DateTime.UtcNow.Date.AddDays(-30),
            Notes = "RunTo before RunFrom"
        };
        var response = await Client.PostAsJsonAsync(PayRunsBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPayRunById_Existing_ReturnsOk()
    {
        var payRunId = await CreateTestPayRunAsync();
        var response = await Client.GetAsync($"{PayRunsBaseUrl}/{payRunId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PayRunDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(payRunId);
    }

    [Fact]
    public async Task GetPayRunById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{PayRunsBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPayRuns_Paged_ReturnsOk()
    {
        await CreateTestPayRunAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{PayRunsBaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<PayRunDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddPayslip_ValidData_ReturnsOk()
    {
        var payRunId = await CreateTestPayRunAsync();
        var staffId = await CreateTestStaffAsync();
        var command = new { PayRunId = payRunId, StaffId = staffId };
        var response = await Client.PostAsJsonAsync($"{PayRunsBaseUrl}/{payRunId}/payslips", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task RemovePayslip_ValidData_ReturnsOk()
    {
        var payRunId = await CreateTestPayRunAsync();
        var staffId = await CreateTestStaffAsync();
        await Client.PostAsJsonAsync($"{PayRunsBaseUrl}/{payRunId}/payslips",
            new { PayRunId = payRunId, StaffId = staffId });
        var response = await Client.DeleteAsync($"{PayRunsBaseUrl}/{payRunId}/payslips/{staffId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ComputePayRun_ReturnsOk()
    {
        var payRunId = await CreateTestPayRunAsync();
        var response = await Client.PostAsJsonAsync($"{PayRunsBaseUrl}/{payRunId}/compute", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ApprovePayRun_ReturnsOk()
    {
        var payRunId = await CreateTestPayRunAsync();
        var response = await Client.PostAsJsonAsync($"{PayRunsBaseUrl}/{payRunId}/approve", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RejectPayRun_ReturnsOk()
    {
        var payRunId = await CreateTestPayRunAsync();
        var response = await Client.PostAsJsonAsync($"{PayRunsBaseUrl}/{payRunId}/reject", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RestartPayRun_ReturnsOk()
    {
        var payRunId = await CreateTestPayRunAsync();
        var response = await Client.PostAsJsonAsync($"{PayRunsBaseUrl}/{payRunId}/restart", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PayPayRun_ReturnsOk()
    {
        var payRunId = await CreateTestPayRunAsync();
        var response = await Client.PostAsJsonAsync($"{PayRunsBaseUrl}/{payRunId}/pay", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeletePayRun_ReturnsOk()
    {
        var payRunId = await CreateTestPayRunAsync();
        var response = await Client.DeleteAsync($"{PayRunsBaseUrl}/{payRunId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeletePayRun_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{PayRunsBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Salaries

    [Fact]
    public async Task CreateSalary_ValidData_ReturnsOk()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            BaseSalary = 15000m,
            SalaryType = "Monthly",
            EffectiveDate = DateTime.UtcNow.Date.AddDays(-10)
        };
        var response = await Client.PostAsJsonAsync(SalariesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateSalary_MissingStaffId_ReturnsBadRequest()
    {
        var command = new
        {
            StaffId = Guid.Empty,
            BaseSalary = 15000m,
            SalaryType = "Monthly",
            EffectiveDate = DateTime.UtcNow.Date.AddDays(-10)
        };
        var response = await Client.PostAsJsonAsync(SalariesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSalary_NegativeBaseSalary_ReturnsBadRequest()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            BaseSalary = -5000m,
            SalaryType = "Monthly",
            EffectiveDate = DateTime.UtcNow.Date.AddDays(-10)
        };
        var response = await Client.PostAsJsonAsync(SalariesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSalaryById_Existing_ReturnsOk()
    {
        var salaryId = await CreateTestSalaryAsync();
        var response = await Client.GetAsync($"{SalariesBaseUrl}/{salaryId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SalaryDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(salaryId);
    }

    [Fact]
    public async Task GetSalaryById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{SalariesBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSalaries_Paged_ReturnsOk()
    {
        await CreateTestSalaryAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{SalariesBaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<SalaryDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateSalary_ValidData_ReturnsOk()
    {
        var salaryId = await CreateTestSalaryAsync();
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            SalaryId = salaryId,
            StaffId = staffId,
            BaseSalary = 18000m,
            SalaryType = "Monthly",
            EffectiveDate = DateTime.UtcNow.Date.AddDays(-5)
        };
        var response = await Client.PutAsJsonAsync($"{SalariesBaseUrl}/{salaryId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ActivateSalary_ReturnsOk()
    {
        var salaryId = await CreateTestSalaryAsync();
        var response = await Client.PostAsJsonAsync($"{SalariesBaseUrl}/{salaryId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeactivateSalary_ReturnsOk()
    {
        var salaryId = await CreateTestSalaryAsync();
        var response = await Client.PostAsJsonAsync($"{SalariesBaseUrl}/{salaryId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteSalary_ReturnsOk()
    {
        var salaryId = await CreateTestSalaryAsync();
        var response = await Client.DeleteAsync($"{SalariesBaseUrl}/{salaryId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteSalary_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{SalariesBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region SalaryComponents

    [Fact]
    public async Task CreateSalaryComponent_ValidData_ReturnsOk()
    {
        var command = new
        {
            Name = $"Transport_{Guid.NewGuid():N}",
            Kind = "Earning",
            IsPercentage = false,
            DefaultValue = 500m,
            Frequency = "Monthly",
            IsPerWorkDay = false
        };
        var response = await Client.PostAsJsonAsync(SalaryComponentsBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateSalaryComponent_MissingName_ReturnsBadRequest()
    {
        var command = new
        {
            Name = "",
            Kind = "Earning",
            IsPercentage = false,
            DefaultValue = 500m
        };
        var response = await Client.PostAsJsonAsync(SalaryComponentsBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSalaryComponent_MissingKind_ReturnsBadRequest()
    {
        var command = new
        {
            Name = $"Bonus_{Guid.NewGuid():N}",
            Kind = "",
            IsPercentage = false,
            DefaultValue = 1000m
        };
        var response = await Client.PostAsJsonAsync(SalaryComponentsBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSalaryComponentById_Existing_ReturnsOk()
    {
        var componentId = await CreateTestSalaryComponentAsync();
        var response = await Client.GetAsync($"{SalaryComponentsBaseUrl}/{componentId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SalaryComponentDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(componentId);
    }

    [Fact]
    public async Task GetSalaryComponentById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{SalaryComponentsBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSalaryComponents_Paged_ReturnsOk()
    {
        await CreateTestSalaryComponentAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{SalaryComponentsBaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<SalaryComponentDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DeleteSalaryComponent_ReturnsOk()
    {
        var componentId = await CreateTestSalaryComponentAsync();
        var response = await Client.DeleteAsync($"{SalaryComponentsBaseUrl}/{componentId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region ExaminationFees

    [Fact]
    public async Task CreateExaminationFee_ValidData_ReturnsOk()
    {
        var examTypeId = await CreateTestExaminationTypeAsync();
        var command = new
        {
            ExaminationTypeId = examTypeId,
            Role = "Radiologist",
            Amount = 50m,
            IsPercentage = false
        };
        var response = await Client.PostAsJsonAsync(ExaminationFeesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateExaminationFee_MissingExaminationTypeId_ReturnsBadRequest()
    {
        var command = new
        {
            ExaminationTypeId = Guid.Empty,
            Role = "Radiologist",
            Amount = 50m,
            IsPercentage = false
        };
        var response = await Client.PostAsJsonAsync(ExaminationFeesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetExaminationFeeById_Existing_ReturnsOk()
    {
        var feeId = await CreateTestExaminationFeeAsync();
        var response = await Client.GetAsync($"{ExaminationFeesBaseUrl}/{feeId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationFeeDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(feeId);
    }

    [Fact]
    public async Task GetExaminationFeeById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{ExaminationFeesBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetExaminationFees_Paged_ReturnsOk()
    {
        await CreateTestExaminationFeeAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{ExaminationFeesBaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ExaminationFeeDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DeleteExaminationFee_ReturnsOk()
    {
        var feeId = await CreateTestExaminationFeeAsync();
        var response = await Client.DeleteAsync($"{ExaminationFeesBaseUrl}/{feeId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region ReferralFees

    [Fact]
    public async Task CreateReferralFee_ValidData_ReturnsOk()
    {
        var referralDoctorId = await CreateTestReferralDoctorAsync();
        var examTypeId = await CreateTestExaminationTypeAsync();
        var command = new
        {
            ReferralDoctorId = referralDoctorId,
            ExaminationTypeId = examTypeId,
            Amount = 100m,
            IsPercentage = false
        };
        var response = await Client.PostAsJsonAsync(ReferralFeesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateReferralFee_MissingReferralDoctorId_ReturnsBadRequest()
    {
        var examTypeId = await CreateTestExaminationTypeAsync();
        var command = new
        {
            ReferralDoctorId = Guid.Empty,
            ExaminationTypeId = examTypeId,
            Amount = 100m,
            IsPercentage = false
        };
        var response = await Client.PostAsJsonAsync(ReferralFeesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetReferralFeeById_Existing_ReturnsOk()
    {
        var feeId = await CreateTestReferralFeeAsync();
        var response = await Client.GetAsync($"{ReferralFeesBaseUrl}/{feeId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReferralFeeDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(feeId);
    }

    [Fact]
    public async Task GetReferralFeeById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{ReferralFeesBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReferralFees_Paged_ReturnsOk()
    {
        await CreateTestReferralFeeAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{ReferralFeesBaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ReferralFeeDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DeleteReferralFee_ReturnsOk()
    {
        var feeId = await CreateTestReferralFeeAsync();
        var response = await Client.DeleteAsync($"{ReferralFeesBaseUrl}/{feeId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Allowances

    [Fact]
    public async Task CreateAllowance_ValidData_ReturnsOk()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            Name = $"Transport_{Guid.NewGuid():N}",
            Amount = 500m,
            EffectiveDate = DateTime.UtcNow.Date.AddDays(-10),
            Frequency = "Monthly",
            IsPerWorkDay = false
        };
        var response = await Client.PostAsJsonAsync(AllowancesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse>();
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAllowance_MissingStaffId_ReturnsBadRequest()
    {
        var command = new
        {
            StaffId = Guid.Empty,
            Name = $"Transport_{Guid.NewGuid():N}",
            Amount = 500m,
            EffectiveDate = DateTime.UtcNow.Date.AddDays(-10)
        };
        var response = await Client.PostAsJsonAsync(AllowancesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAllowance_MissingName_ReturnsBadRequest()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            Name = "",
            Amount = 500m,
            EffectiveDate = DateTime.UtcNow.Date.AddDays(-10)
        };
        var response = await Client.PostAsJsonAsync(AllowancesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAllowanceById_Existing_ReturnsOk()
    {
        var allowanceId = await CreateTestAllowanceAsync();
        var response = await Client.GetAsync($"{AllowancesBaseUrl}/{allowanceId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<AllowanceAssignmentDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(allowanceId);
    }

    [Fact]
    public async Task GetAllowanceById_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{AllowancesBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllowances_Paged_ReturnsOk()
    {
        await CreateTestAllowanceAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{AllowancesBaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<AllowanceAssignmentDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAllowance_ValidData_ReturnsOk()
    {
        var allowanceId = await CreateTestAllowanceAsync();
        var command = new
        {
            AllowanceAssignmentId = allowanceId,
            Name = $"Updated_{Guid.NewGuid():N}",
            Amount = 750m,
            EffectiveDate = DateTime.UtcNow.Date.AddDays(-5),
            Frequency = "Monthly",
            IsPerWorkDay = false
        };
        var response = await Client.PutAsJsonAsync($"{AllowancesBaseUrl}/{allowanceId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ActivateAllowance_ReturnsOk()
    {
        var allowanceId = await CreateTestAllowanceAsync();
        var response = await Client.PostAsJsonAsync($"{AllowancesBaseUrl}/{allowanceId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeactivateAllowance_ReturnsOk()
    {
        var allowanceId = await CreateTestAllowanceAsync();
        var response = await Client.PostAsJsonAsync($"{AllowancesBaseUrl}/{allowanceId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteAllowance_ReturnsOk()
    {
        var allowanceId = await CreateTestAllowanceAsync();
        var response = await Client.DeleteAsync($"{AllowancesBaseUrl}/{allowanceId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteAllowance_Nonexistent_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{AllowancesBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Helpers

    private async Task<Guid> CreateTestStaffAsync()
    {
        var command = new
        {
            UserId = Guid.NewGuid(),
            FullName = $"Test Staff {Guid.NewGuid():N}",
            PhoneNumber = $"010{Random.Shared.Next(10000000, 99999999)}",
            Position = "Technician",
            HireDate = DateTime.UtcNow.Date.AddDays(-5)
        };
        var response = await Client.PostAsJsonAsync(StaffBaseUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var responseBody = await response.Content.ReadFromJsonAsync<ApiResponse<StaffDto>>();
        return responseBody!.Data!.Id;
    }

    private async Task<Guid> CreateTestReferralDoctorAsync()
    {
        var command = new
        {
            FullName = $"Dr. Test {Guid.NewGuid():N}",
            Phone = $"010{Random.Shared.Next(10000000, 99999999)}"
        };
        var response = await Client.PostAsJsonAsync(ReferralDoctorsBaseUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var responseBody = await response.Content.ReadFromJsonAsync<ApiResponse<ReferralDoctorDto>>();
        return responseBody!.Data!.Id;
    }

    private async Task<Guid> CreateTestExaminationTypeAsync()
    {
        var name = $"TestExType_{Guid.NewGuid():N}";
        var command = new
        {
            Name = name,
            Modality = "XRay",
            BodyPart = "Chest",
            StandardDurationMinutes = 15,
            Price = 250m,
            RequiresPreparation = false,
            RequiresConsent = false
        };
        var createResponse = await Client.PostAsJsonAsync(ExaminationTypesBaseUrl, command);
        if (createResponse.StatusCode != HttpStatusCode.OK)
        {
            var body = await createResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {createResponse.StatusCode}: {body}");
        }
        var allResponse = await Client.PostAsJsonAsync($"{ExaminationTypesBaseUrl}/all",
            new { Pagination = new { PageNumber = 1, PageSize = 1 }, SearchTerm = name });
        if (allResponse.StatusCode != HttpStatusCode.OK)
        {
            var body = await allResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {allResponse.StatusCode}: {body}");
        }
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ExaminationTypeDto>>>();
        return allBody!.Data!.Items.First().Id;
    }

    private async Task<Guid> CreateTestPayRunAsync()
    {
        var command = new
        {
            RunFrom = DateTime.UtcNow.Date.AddDays(-30),
            RunTo = DateTime.UtcNow.Date,
            Notes = "Test pay run"
        };
        var response = await Client.PostAsJsonAsync(PayRunsBaseUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var responseBody = await response.Content.ReadFromJsonAsync<ApiResponse<PayRunDto>>();
        return responseBody!.Data!.Id;
    }

    private async Task<Guid> CreateTestSalaryAsync()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            BaseSalary = 15000m,
            SalaryType = "Monthly",
            EffectiveDate = DateTime.UtcNow.Date.AddDays(-10)
        };
        var response = await Client.PostAsJsonAsync(SalariesBaseUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var responseBody = await response.Content.ReadFromJsonAsync<ApiResponse<SalaryDto>>();
        return responseBody!.Data!.Id;
    }

    private async Task<Guid> CreateTestSalaryComponentAsync()
    {
        var command = new
        {
            Name = $"Transport_{Guid.NewGuid():N}",
            Kind = "Earning",
            IsPercentage = false,
            DefaultValue = 500m,
            Frequency = "Monthly",
            IsPerWorkDay = false
        };
        var response = await Client.PostAsJsonAsync(SalaryComponentsBaseUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var responseBody = await response.Content.ReadFromJsonAsync<ApiResponse<SalaryComponentDto>>();
        return responseBody!.Data!.Id;
    }

    private async Task<Guid> CreateTestExaminationFeeAsync()
    {
        var examTypeId = await CreateTestExaminationTypeAsync();
        var command = new
        {
            ExaminationTypeId = examTypeId,
            Role = "Radiologist",
            Amount = 50m,
            IsPercentage = false
        };
        var response = await Client.PostAsJsonAsync(ExaminationFeesBaseUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var responseBody = await response.Content.ReadFromJsonAsync<ApiResponse<ExaminationFeeDto>>();
        return responseBody!.Data!.Id;
    }

    private async Task<Guid> CreateTestReferralFeeAsync()
    {
        var referralDoctorId = await CreateTestReferralDoctorAsync();
        var examTypeId = await CreateTestExaminationTypeAsync();
        var command = new
        {
            ReferralDoctorId = referralDoctorId,
            ExaminationTypeId = examTypeId,
            Amount = 100m,
            IsPercentage = false
        };
        var response = await Client.PostAsJsonAsync(ReferralFeesBaseUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var responseBody = await response.Content.ReadFromJsonAsync<ApiResponse<ReferralFeeDto>>();
        return responseBody!.Data!.Id;
    }

    private async Task<Guid> CreateTestAllowanceAsync()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            Name = $"Transport_{Guid.NewGuid():N}",
            Amount = 500m,
            EffectiveDate = DateTime.UtcNow.Date.AddDays(-10),
            Frequency = "Monthly",
            IsPerWorkDay = false
        };
        var response = await Client.PostAsJsonAsync(AllowancesBaseUrl, command);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected OK but got {response.StatusCode}: {body}");
        }
        var responseBody = await response.Content.ReadFromJsonAsync<ApiResponse<AllowanceAssignmentDto>>();
        return responseBody!.Data!.Id;
    }

    #endregion

    #region DTOs

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

    private sealed class PayRunDto
    {
        public Guid Id { get; set; }
        public DateTime RunFrom { get; set; }
        public DateTime RunTo { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    private sealed class SalaryDto
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public decimal BaseSalary { get; set; }
        public string? SalaryType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class SalaryComponentDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Kind { get; set; }
        public bool IsPercentage { get; set; }
        public decimal DefaultValue { get; set; }
        public string? Frequency { get; set; }
        public bool IsPerWorkDay { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class ExaminationFeeDto
    {
        public Guid Id { get; set; }
        public Guid ExaminationTypeId { get; set; }
        public string? Role { get; set; }
        public decimal Amount { get; set; }
        public bool IsPercentage { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class ReferralFeeDto
    {
        public Guid Id { get; set; }
        public Guid ReferralDoctorId { get; set; }
        public Guid ExaminationTypeId { get; set; }
        public decimal Amount { get; set; }
        public bool IsPercentage { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class AllowanceAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public string? Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Guid? SalaryComponentId { get; set; }
        public string? Frequency { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPerWorkDay { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class StaffDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Position { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class ReferralDoctorDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class ExaminationTypeDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Modality { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    #endregion
}
