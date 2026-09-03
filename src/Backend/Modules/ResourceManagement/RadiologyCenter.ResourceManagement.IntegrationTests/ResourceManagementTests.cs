using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class ResourceManagementTests : TestBase
{
    private const string StaffBaseUrl = "api/resources/staff";
    private const string EquipmentBaseUrl = "api/resources/equipment";
    private const string ReferralDoctorsBaseUrl = "api/resources/referral-doctors";
    private const string LeavesBaseUrl = "api/resources/leaves";
    private const string WorkShiftsBaseUrl = "api/resources/work-shifts";

    public ResourceManagementTests(CustomWebApplicationFactory factory) : base(factory) { }

    #region Staff

    [Fact]
    public async Task CreateStaff_ValidCommand_ReturnsOk()
    {
        var command = new
        {
            UserId = Guid.NewGuid(),
            FullName = "Ahmed Mohamed Ali",
            PhoneNumber = "01012345678",
            Position = "Technician",
            HireDate = DateTime.UtcNow.Date.AddDays(-10)
        };
        var response = await Client.PostAsJsonAsync(StaffBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<StaffDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.FullName.Should().Be(command.FullName);
        body.Data.Position.Should().Be(command.Position);
    }

    [Fact]
    public async Task CreateStaff_MissingRequiredFields_ReturnsBadRequest()
    {
        var command = new { };
        var response = await Client.PostAsJsonAsync(StaffBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateStaff_InvalidPosition_ReturnsBadRequest()
    {
        var command = new
        {
            UserId = Guid.NewGuid(),
            FullName = "Ahmed Mohamed Ali",
            PhoneNumber = "01012345678",
            Position = "InvalidPosition",
            HireDate = DateTime.UtcNow.Date
        };
        var response = await Client.PostAsJsonAsync(StaffBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetStaffById_ExistingStaff_ReturnsOk()
    {
        var staffId = await CreateTestStaffAsync();
        var response = await Client.GetAsync($"{StaffBaseUrl}/{staffId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<StaffDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(staffId);
    }

    [Fact]
    public async Task GetStaffById_NonexistentStaff_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{StaffBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllStaff_ReturnsPagedResult()
    {
        await CreateTestStaffAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{StaffBaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<StaffDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateStaff_ExistingStaff_ReturnsOk()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            UserId = Guid.NewGuid(),
            FullName = "Updated Staff Name",
            PhoneNumber = "01098765432",
            Position = "Radiologist",
            HireDate = DateTime.UtcNow.Date.AddYears(-1)
        };
        var response = await Client.PutAsJsonAsync($"{StaffBaseUrl}/{staffId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateStaff_NonexistentStaff_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new
        {
            UserId = Guid.NewGuid(),
            FullName = "Updated Staff Name",
            PhoneNumber = "01098765432",
            Position = "Radiologist",
            HireDate = DateTime.UtcNow.Date
        };
        var response = await Client.PutAsJsonAsync($"{StaffBaseUrl}/{fakeId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ActivateStaff_DeactivatedStaff_ReturnsOk()
    {
        var staffId = await CreateTestStaffAsync();
        await Client.PostAsJsonAsync($"{StaffBaseUrl}/{staffId}/deactivate", new { });
        var response = await Client.PostAsJsonAsync($"{StaffBaseUrl}/{staffId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeactivateStaff_ExistingStaff_ReturnsOk()
    {
        var staffId = await CreateTestStaffAsync();
        var response = await Client.PostAsJsonAsync($"{StaffBaseUrl}/{staffId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteStaff_ExistingStaff_ReturnsOk()
    {
        var staffId = await CreateTestStaffAsync();
        var response = await Client.DeleteAsync($"{StaffBaseUrl}/{staffId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteStaff_NonexistentStaff_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{StaffBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Equipment

    [Fact]
    public async Task CreateEquipment_ValidCommand_ReturnsOk()
    {
        var command = new
        {
            Name = $"XRay Machine {Guid.NewGuid():N}",
            Modality = "XRay",
            SerialNumber = $"SN-{Guid.NewGuid():N}",
            PurchaseDate = DateTime.UtcNow.Date.AddYears(-1)
        };
        var response = await Client.PostAsJsonAsync(EquipmentBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<EquipmentDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Name.Should().Be(command.Name);
        body.Data.Modality.Should().Be(command.Modality);
    }

    [Fact]
    public async Task CreateEquipment_MissingRequiredFields_ReturnsBadRequest()
    {
        var command = new { };
        var response = await Client.PostAsJsonAsync(EquipmentBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateEquipment_InvalidModality_ReturnsBadRequest()
    {
        var command = new
        {
            Name = "Test Equipment",
            Modality = "InvalidModality"
        };
        var response = await Client.PostAsJsonAsync(EquipmentBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetEquipmentById_ExistingEquipment_ReturnsOk()
    {
        var equipmentId = await CreateTestEquipmentAsync();
        var response = await Client.GetAsync($"{EquipmentBaseUrl}/{equipmentId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<EquipmentDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(equipmentId);
    }

    [Fact]
    public async Task GetEquipmentById_NonexistentEquipment_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{EquipmentBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllEquipment_ReturnsPagedResult()
    {
        await CreateTestEquipmentAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{EquipmentBaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<EquipmentDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateEquipment_ExistingEquipment_ReturnsOk()
    {
        var equipmentId = await CreateTestEquipmentAsync();
        var command = new
        {
            Name = "Updated CT Scanner",
            Modality = "CT",
            SerialNumber = $"SN-UPD-{Guid.NewGuid():N}"
        };
        var response = await Client.PutAsJsonAsync($"{EquipmentBaseUrl}/{equipmentId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SetEquipmentStatus_ValidStatus_ReturnsOk()
    {
        var equipmentId = await CreateTestEquipmentAsync();
        var command = new { Status = "UnderMaintenance" };
        var response = await Client.PostAsJsonAsync($"{EquipmentBaseUrl}/{equipmentId}/status", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ActivateEquipment_DeactivatedEquipment_ReturnsOk()
    {
        var equipmentId = await CreateTestEquipmentAsync();
        await Client.PostAsJsonAsync($"{EquipmentBaseUrl}/{equipmentId}/deactivate", new { });
        var response = await Client.PostAsJsonAsync($"{EquipmentBaseUrl}/{equipmentId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeactivateEquipment_ExistingEquipment_ReturnsOk()
    {
        var equipmentId = await CreateTestEquipmentAsync();
        var response = await Client.PostAsJsonAsync($"{EquipmentBaseUrl}/{equipmentId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteEquipment_ExistingEquipment_ReturnsOk()
    {
        var equipmentId = await CreateTestEquipmentAsync();
        var response = await Client.DeleteAsync($"{EquipmentBaseUrl}/{equipmentId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region ReferralDoctors

    [Fact]
    public async Task CreateReferralDoctor_ValidCommand_ReturnsOk()
    {
        var command = new
        {
            FullName = "Dr. Salah Hassan Ibrahim",
            Phone = "01012345678"
        };
        var response = await Client.PostAsJsonAsync(ReferralDoctorsBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReferralDoctorDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.FullName.Should().Be(command.FullName);
        body.Data.Phone.Should().Be(command.Phone);
    }

    [Fact]
    public async Task CreateReferralDoctor_MissingRequiredFields_ReturnsBadRequest()
    {
        var command = new { };
        var response = await Client.PostAsJsonAsync(ReferralDoctorsBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReferralDoctor_DuplicatePhone_ReturnsConflict()
    {
        var phone = $"010{Random.Shared.Next(10000000, 99999999)}";
        var cmd1 = new { FullName = "Dr. First Doctor Name", Phone = phone };
        var cmd2 = new { FullName = "Dr. Second Doctor Name", Phone = phone };
        await Client.PostAsJsonAsync(ReferralDoctorsBaseUrl, cmd1);
        var response = await Client.PostAsJsonAsync(ReferralDoctorsBaseUrl, cmd2);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetReferralDoctorById_ExistingDoctor_ReturnsOk()
    {
        var doctorId = await CreateTestReferralDoctorAsync();
        var response = await Client.GetAsync($"{ReferralDoctorsBaseUrl}/{doctorId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ReferralDoctorDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(doctorId);
    }

    [Fact]
    public async Task GetReferralDoctorById_NonexistentDoctor_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{ReferralDoctorsBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllReferralDoctors_ReturnsPagedResult()
    {
        await CreateTestReferralDoctorAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{ReferralDoctorsBaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ReferralDoctorDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateReferralDoctor_ExistingDoctor_ReturnsOk()
    {
        var doctorId = await CreateTestReferralDoctorAsync();
        var command = new
        {
            FullName = "Dr. Updated Doctor Name",
            Phone = $"010{Random.Shared.Next(10000000, 99999999)}"
        };
        var response = await Client.PutAsJsonAsync($"{ReferralDoctorsBaseUrl}/{doctorId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateReferralDoctor_NonexistentDoctor_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { FullName = "Dr. Updated Name", Phone = "01098765432" };
        var response = await Client.PutAsJsonAsync($"{ReferralDoctorsBaseUrl}/{fakeId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ActivateReferralDoctor_DeactivatedDoctor_ReturnsOk()
    {
        var doctorId = await CreateTestReferralDoctorAsync();
        await Client.PostAsJsonAsync($"{ReferralDoctorsBaseUrl}/{doctorId}/deactivate", new { });
        var response = await Client.PostAsJsonAsync($"{ReferralDoctorsBaseUrl}/{doctorId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeactivateReferralDoctor_ExistingDoctor_ReturnsOk()
    {
        var doctorId = await CreateTestReferralDoctorAsync();
        var response = await Client.PostAsJsonAsync($"{ReferralDoctorsBaseUrl}/{doctorId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteReferralDoctor_ExistingDoctor_ReturnsOk()
    {
        var doctorId = await CreateTestReferralDoctorAsync();
        var response = await Client.DeleteAsync($"{ReferralDoctorsBaseUrl}/{doctorId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Leaves

    [Fact]
    public async Task CreateLeave_ValidCommand_ReturnsOk()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            LeaveType = "Annual",
            StartDate = DateTime.UtcNow.Date.AddDays(5),
            EndDate = DateTime.UtcNow.Date.AddDays(7)
        };
        var response = await Client.PostAsJsonAsync(LeavesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LeaveDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.StaffId.Should().Be(staffId);
        body.Data.LeaveType.Should().Be(command.LeaveType);
    }

    [Fact]
    public async Task CreateLeave_MissingStaffId_ReturnsBadRequest()
    {
        var command = new
        {
            StaffId = Guid.Empty,
            LeaveType = "Annual",
            StartDate = DateTime.UtcNow.Date.AddDays(5),
            EndDate = DateTime.UtcNow.Date.AddDays(7)
        };
        var response = await Client.PostAsJsonAsync(LeavesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateLeave_InvalidLeaveType_ReturnsBadRequest()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            LeaveType = "InvalidType",
            StartDate = DateTime.UtcNow.Date.AddDays(5),
            EndDate = DateTime.UtcNow.Date.AddDays(7)
        };
        var response = await Client.PostAsJsonAsync(LeavesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateLeave_EndDateBeforeStartDate_ReturnsBadRequest()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            LeaveType = "Annual",
            StartDate = DateTime.UtcNow.Date.AddDays(10),
            EndDate = DateTime.UtcNow.Date.AddDays(5)
        };
        var response = await Client.PostAsJsonAsync(LeavesBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateLeave_OverlappingLeave_ReturnsConflict()
    {
        var staffId = await CreateTestStaffAsync();
        var cmd1 = new
        {
            StaffId = staffId,
            LeaveType = "Annual",
            StartDate = DateTime.UtcNow.Date.AddDays(10),
            EndDate = DateTime.UtcNow.Date.AddDays(15)
        };
        await Client.PostAsJsonAsync(LeavesBaseUrl, cmd1);

        var cmd2 = new
        {
            StaffId = staffId,
            LeaveType = "Sick",
            StartDate = DateTime.UtcNow.Date.AddDays(12),
            EndDate = DateTime.UtcNow.Date.AddDays(18)
        };
        var response = await Client.PostAsJsonAsync(LeavesBaseUrl, cmd2);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetLeaveById_ExistingLeave_ReturnsOk()
    {
        var leaveId = await CreateTestLeaveAsync();
        var response = await Client.GetAsync($"{LeavesBaseUrl}/{leaveId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LeaveDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(leaveId);
    }

    [Fact]
    public async Task GetLeaveById_NonexistentLeave_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{LeavesBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllLeaves_ReturnsPagedResult()
    {
        await CreateTestLeaveAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{LeavesBaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<LeaveDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateLeave_ExistingLeave_ReturnsOk()
    {
        var leaveId = await CreateTestLeaveAsync();
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            LeaveType = "Personal",
            StartDate = DateTime.UtcNow.Date.AddDays(20),
            EndDate = DateTime.UtcNow.Date.AddDays(22)
        };
        var response = await Client.PutAsJsonAsync($"{LeavesBaseUrl}/{leaveId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteLeave_ExistingLeave_ReturnsOk()
    {
        var leaveId = await CreateTestLeaveAsync();
        var response = await Client.DeleteAsync($"{LeavesBaseUrl}/{leaveId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region WorkShifts

    [Fact]
    public async Task CreateWorkShift_ValidCommand_ReturnsOk()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            Date = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(16, 0, 0)
        };
        var response = await Client.PostAsJsonAsync(WorkShiftsBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<WorkShiftDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.StaffId.Should().Be(staffId);
        body.Data.StartTime.Should().Be(command.StartTime);
        body.Data.EndTime.Should().Be(command.EndTime);
    }

    [Fact]
    public async Task CreateWorkShift_EndTimeBeforeStartTime_ReturnsBadRequest()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            Date = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(16, 0, 0),
            EndTime = new TimeSpan(8, 0, 0)
        };
        var response = await Client.PostAsJsonAsync(WorkShiftsBaseUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateWorkShift_OverlappingShift_ReturnsConflict()
    {
        var staffId = await CreateTestStaffAsync();
        var shiftDate = DateTime.UtcNow.Date.AddDays(2);
        var cmd1 = new
        {
            StaffId = staffId,
            Date = shiftDate,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(16, 0, 0)
        };
        await Client.PostAsJsonAsync(WorkShiftsBaseUrl, cmd1);

        var cmd2 = new
        {
            StaffId = staffId,
            Date = shiftDate,
            StartTime = new TimeSpan(12, 0, 0),
            EndTime = new TimeSpan(20, 0, 0)
        };
        var response = await Client.PostAsJsonAsync(WorkShiftsBaseUrl, cmd2);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetWorkShiftById_ExistingShift_ReturnsOk()
    {
        var shiftId = await CreateTestWorkShiftAsync();
        var response = await Client.GetAsync($"{WorkShiftsBaseUrl}/{shiftId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<WorkShiftDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(shiftId);
    }

    [Fact]
    public async Task GetWorkShiftById_NonexistentShift_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{WorkShiftsBaseUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllWorkShifts_ReturnsPagedResult()
    {
        await CreateTestWorkShiftAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{WorkShiftsBaseUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<WorkShiftDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateWorkShift_ExistingShift_ReturnsOk()
    {
        var shiftId = await CreateTestWorkShiftAsync();
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            Date = DateTime.UtcNow.Date.AddDays(3),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(17, 0, 0)
        };
        var response = await Client.PutAsJsonAsync($"{WorkShiftsBaseUrl}/{shiftId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateWorkShift_NonexistentShift_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            Date = DateTime.UtcNow.Date.AddDays(3),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(17, 0, 0)
        };
        var response = await Client.PutAsJsonAsync($"{WorkShiftsBaseUrl}/{fakeId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteWorkShift_ExistingShift_ReturnsOk()
    {
        var shiftId = await CreateTestWorkShiftAsync();
        var response = await Client.DeleteAsync($"{WorkShiftsBaseUrl}/{shiftId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
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
        response.EnsureSuccessStatusCode();
        var allResponse = await Client.PostAsJsonAsync($"{StaffBaseUrl}/all",
            new { Pagination = new { PageNumber = 1, PageSize = 1 }, SearchTerm = command.FullName });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<StaffDto>>>();
        return allBody!.Data!.Items.First().Id;
    }

    private async Task<Guid> CreateTestEquipmentAsync()
    {
        var command = new
        {
            Name = $"Test Equipment {Guid.NewGuid():N}",
            Modality = "CT",
            SerialNumber = $"SN-{Guid.NewGuid():N}"
        };
        var response = await Client.PostAsJsonAsync(EquipmentBaseUrl, command);
        response.EnsureSuccessStatusCode();
        var allResponse = await Client.PostAsJsonAsync($"{EquipmentBaseUrl}/all",
            new { Pagination = new { PageNumber = 1, PageSize = 1 }, SearchTerm = command.Name });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<EquipmentDto>>>();
        return allBody!.Data!.Items.First().Id;
    }

    private async Task<Guid> CreateTestReferralDoctorAsync()
    {
        var command = new
        {
            FullName = $"Dr. Test Doctor {Guid.NewGuid():N}",
            Phone = $"010{Random.Shared.Next(10000000, 99999999)}"
        };
        var response = await Client.PostAsJsonAsync(ReferralDoctorsBaseUrl, command);
        response.EnsureSuccessStatusCode();
        var allResponse = await Client.PostAsJsonAsync($"{ReferralDoctorsBaseUrl}/all",
            new { Pagination = new { PageNumber = 1, PageSize = 1 }, SearchTerm = command.FullName });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ReferralDoctorDto>>>();
        return allBody!.Data!.Items.First().Id;
    }

    private async Task<Guid> CreateTestLeaveAsync()
    {
        var staffId = await CreateTestStaffAsync();
        var command = new
        {
            StaffId = staffId,
            LeaveType = "Annual",
            StartDate = DateTime.UtcNow.Date.AddDays(30),
            EndDate = DateTime.UtcNow.Date.AddDays(35)
        };
        var response = await Client.PostAsJsonAsync(LeavesBaseUrl, command);
        response.EnsureSuccessStatusCode();
        var allResponse = await Client.PostAsJsonAsync($"{LeavesBaseUrl}/all",
            new { Pagination = new { PageNumber = 1, PageSize = 1 } });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<LeaveDto>>>();
        return allBody!.Data!.Items.First().Id;
    }

    private async Task<Guid> CreateTestWorkShiftAsync()
    {
        var staffId = await CreateTestStaffAsync();
        var shiftDate = DateTime.UtcNow.Date.AddDays(4);
        var command = new
        {
            StaffId = staffId,
            Date = shiftDate,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(16, 0, 0)
        };
        var response = await Client.PostAsJsonAsync(WorkShiftsBaseUrl, command);
        response.EnsureSuccessStatusCode();
        var allResponse = await Client.PostAsJsonAsync($"{WorkShiftsBaseUrl}/all",
            new { Pagination = new { PageNumber = 1, PageSize = 1 } });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<WorkShiftDto>>>();
        return allBody!.Data!.Items.First().Id;
    }

    #endregion

    #region DTOs

    private sealed class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }

    private sealed class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    private sealed class StaffDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Position { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class EquipmentDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Modality { get; set; }
        public string? SerialNumber { get; set; }
        public string? Status { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class ReferralDoctorDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Specialization { get; set; }
        public string? Hospital { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class LeaveDto
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public string? LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }

    private sealed class WorkShiftDto
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public Guid? EquipmentId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Notes { get; set; }
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
