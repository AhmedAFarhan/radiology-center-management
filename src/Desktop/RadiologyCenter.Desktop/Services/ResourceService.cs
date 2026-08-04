using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class ResourceService
{
    private readonly ApiClient _api;

    public ResourceService(ApiClient api) => _api = api;

    private static object BuildQuery(string? searchTerm, string? sortBy, bool sortDescending, int pageNumber, int pageSize)
        => new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

    // ----- Equipment -----
    public Task<PagedResult<EquipmentDto>> GetEquipmentPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => _api.PostAsync<PagedResult<EquipmentDto>>("api/resources/equipment/all", BuildQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    public Task<EquipmentDto> GetEquipmentByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<EquipmentDto>($"api/resources/equipment/{id}", ct);

    public Task<EquipmentDto> CreateEquipmentAsync(EquipmentInput input, CancellationToken ct = default)
        => _api.PostAsync<EquipmentDto>("api/resources/equipment", input, ct);

    public Task UpdateEquipmentAsync(string id, EquipmentInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/resources/equipment/{id}", input, ct);

    public Task SetEquipmentStatusAsync(string id, string status, CancellationToken ct = default)
        => _api.PostAsync<object>($"api/resources/equipment/{id}/status", new { status }, ct);

    public Task ActivateEquipmentAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/resources/equipment/{id}/activate", ct: ct);

    public Task DeactivateEquipmentAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/resources/equipment/{id}/deactivate", ct: ct);

    public Task DeleteEquipmentAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/resources/equipment/{id}", ct);

    // ----- Staff -----
    public Task<PagedResult<StaffDto>> GetStaffsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => _api.PostAsync<PagedResult<StaffDto>>("api/resources/staff/all", BuildQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    public Task<StaffDto> GetStaffByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<StaffDto>($"api/resources/staff/{id}", ct);

    public Task<StaffDto> CreateStaffAsync(StaffInput input, CancellationToken ct = default)
        => _api.PostAsync<StaffDto>("api/resources/staff", input, ct);

    public Task UpdateStaffAsync(string id, StaffInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/resources/staff/{id}", input, ct);

    public Task ActivateStaffAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/resources/staff/{id}/activate", ct: ct);

    public Task DeactivateStaffAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/resources/staff/{id}/deactivate", ct: ct);

    public Task DeleteStaffAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/resources/staff/{id}", ct);

    // ----- WorkShifts -----
    public Task<PagedResult<WorkShiftDto>> GetWorkShiftsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => _api.PostAsync<PagedResult<WorkShiftDto>>("api/resources/work-shifts/all", BuildQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    public Task<WorkShiftDto> GetWorkShiftByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<WorkShiftDto>($"api/resources/work-shifts/{id}", ct);

    public Task<WorkShiftDto> CreateWorkShiftAsync(WorkShiftInput input, CancellationToken ct = default)
        => _api.PostAsync<WorkShiftDto>("api/resources/work-shifts", input, ct);

    public Task UpdateWorkShiftAsync(string id, WorkShiftInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/resources/work-shifts/{id}", input, ct);

    public Task DeleteWorkShiftAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/resources/work-shifts/{id}", ct);

    // ----- Leaves -----
    public Task<PagedResult<LeaveDto>> GetLeavesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => _api.PostAsync<PagedResult<LeaveDto>>("api/resources/leaves/all", BuildQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    public Task<LeaveDto> GetLeaveByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<LeaveDto>($"api/resources/leaves/{id}", ct);

    public Task<LeaveDto> CreateLeaveAsync(LeaveInput input, CancellationToken ct = default)
        => _api.PostAsync<LeaveDto>("api/resources/leaves", input, ct);

    public Task UpdateLeaveAsync(string id, LeaveInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/resources/leaves/{id}", input, ct);

    public Task DeleteLeaveAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/resources/leaves/{id}", ct);

    // ----- Referral Doctors -----
    public Task<PagedResult<ReferralDoctorDto>> GetReferralDoctorsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => _api.PostAsync<PagedResult<ReferralDoctorDto>>("api/resources/referral-doctors/all", BuildQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    public Task<ReferralDoctorDto> GetReferralDoctorByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<ReferralDoctorDto>($"api/resources/referral-doctors/{id}", ct);

    public Task<ReferralDoctorDto> CreateReferralDoctorAsync(ReferralDoctorInput input, CancellationToken ct = default)
        => _api.PostAsync<ReferralDoctorDto>("api/resources/referral-doctors", input, ct);

    public Task UpdateReferralDoctorAsync(string id, ReferralDoctorInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/resources/referral-doctors/{id}", input, ct);

    public Task ActivateReferralDoctorAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/resources/referral-doctors/{id}/activate", ct: ct);

    public Task DeactivateReferralDoctorAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/resources/referral-doctors/{id}/deactivate", ct: ct);

    public Task DeleteReferralDoctorAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/resources/referral-doctors/{id}", ct);
}