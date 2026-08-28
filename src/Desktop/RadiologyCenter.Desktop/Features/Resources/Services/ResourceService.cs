using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Resources.Services;

public sealed class ResourceService : CrudServiceBase
{
    private const string Base = "api/resources";
    private const string EquipmentRes = $"{Base}/equipment";
    private const string StaffRes = $"{Base}/staff";
    private const string WorkShiftsRes = $"{Base}/work-shifts";
    private const string LeavesRes = $"{Base}/leaves";
    private const string ReferralDoctorsRes = $"{Base}/referral-doctors";

    public ResourceService(ApiClient api) : base(api) { }

    // ----- Equipment -----
    public Task<PagedResult<EquipmentDto>> GetEquipmentPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<EquipmentDto>(EquipmentRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<EquipmentDto> GetEquipmentByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<EquipmentDto>(EquipmentRes, id, ct);

    public Task<EquipmentDto> CreateEquipmentAsync(EquipmentInput input, CancellationToken ct = default)
        => CreateEntityAsync<EquipmentDto>(EquipmentRes, input, ct);

    public Task UpdateEquipmentAsync(string id, EquipmentInput input, CancellationToken ct = default)
        => UpdateEntityAsync(EquipmentRes, id, input, ct);

    public Task SetEquipmentStatusAsync(string id, string status, CancellationToken ct = default)
        => Api.PostAsync<object>($"{EquipmentRes}/{id}/status", new { status }, ct);

    public Task ActivateEquipmentAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(EquipmentRes, id, true, ct);

    public Task DeactivateEquipmentAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(EquipmentRes, id, false, ct);

    public Task DeleteEquipmentAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(EquipmentRes, id, ct);

    // ----- Staff -----
    public Task<PagedResult<StaffDto>> GetStaffsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<StaffDto>(StaffRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<StaffDto> GetStaffByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<StaffDto>(StaffRes, id, ct);

    public Task<StaffDto> CreateStaffAsync(StaffInput input, CancellationToken ct = default)
        => CreateEntityAsync<StaffDto>(StaffRes, input, ct);

    public Task UpdateStaffAsync(string id, StaffInput input, CancellationToken ct = default)
        => UpdateEntityAsync(StaffRes, id, input, ct);

    public Task ActivateStaffAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(StaffRes, id, true, ct);

    public Task DeactivateStaffAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(StaffRes, id, false, ct);

    public Task DeleteStaffAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(StaffRes, id, ct);

    // ----- WorkShifts -----
    public Task<PagedResult<WorkShiftDto>> GetWorkShiftsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<WorkShiftDto>(WorkShiftsRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<WorkShiftDto> GetWorkShiftByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<WorkShiftDto>(WorkShiftsRes, id, ct);

    public Task<WorkShiftDto> CreateWorkShiftAsync(WorkShiftInput input, CancellationToken ct = default)
        => CreateEntityAsync<WorkShiftDto>(WorkShiftsRes, input, ct);

    public Task UpdateWorkShiftAsync(string id, WorkShiftInput input, CancellationToken ct = default)
        => UpdateEntityAsync(WorkShiftsRes, id, input, ct);

    public Task DeleteWorkShiftAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(WorkShiftsRes, id, ct);

    // ----- Leaves -----
    public Task<PagedResult<LeaveDto>> GetLeavesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<LeaveDto>(LeavesRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<LeaveDto> GetLeaveByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<LeaveDto>(LeavesRes, id, ct);

    public Task<LeaveDto> CreateLeaveAsync(LeaveInput input, CancellationToken ct = default)
        => CreateEntityAsync<LeaveDto>(LeavesRes, input, ct);

    public Task UpdateLeaveAsync(string id, LeaveInput input, CancellationToken ct = default)
        => UpdateEntityAsync(LeavesRes, id, input, ct);

    public Task DeleteLeaveAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(LeavesRes, id, ct);

    // ----- Referral Doctors -----
    public Task<PagedResult<ReferralDoctorDto>> GetReferralDoctorsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<ReferralDoctorDto>(ReferralDoctorsRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<ReferralDoctorDto> GetReferralDoctorByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<ReferralDoctorDto>(ReferralDoctorsRes, id, ct);

    public Task<ReferralDoctorDto> CreateReferralDoctorAsync(ReferralDoctorInput input, CancellationToken ct = default)
        => CreateEntityAsync<ReferralDoctorDto>(ReferralDoctorsRes, input, ct);

    public Task UpdateReferralDoctorAsync(string id, ReferralDoctorInput input, CancellationToken ct = default)
        => UpdateEntityAsync(ReferralDoctorsRes, id, input, ct);

    public Task ActivateReferralDoctorAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(ReferralDoctorsRes, id, true, ct);

    public Task DeactivateReferralDoctorAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(ReferralDoctorsRes, id, false, ct);

    public Task DeleteReferralDoctorAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(ReferralDoctorsRes, id, ct);
}
