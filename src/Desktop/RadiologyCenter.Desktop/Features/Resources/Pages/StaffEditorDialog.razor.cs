using System.Net.Http;
using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MudBlazor;
using RadiologyCenter.Desktop;
using RadiologyCenter.Desktop.Components;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Features.Resources.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Resources.Pages;

public partial class StaffEditorDialog : EditorDialogBase
{
    [Parameter] public StaffDto? Staff { get; set; }

    private IReadOnlyList<EnumOptionDto> _positionOptions = Array.Empty<EnumOptionDto>();

    private readonly StaffFormModel _model = new();
    private EditContext _editContext = default!;
    private UserDto? _selectedUser;

    private bool IsEdit => Staff is not null;

    private async Task<IEnumerable<UserDto>> SearchUsersAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<UserDto>();

        var page = await SafeExecute.RunAsync(
            () => IdentityService.GetUsersPagedAsync(value, "LastName", false, 1, 20, ct),
            Snackbar,
            () => T.StaffDialog.SearchError);
        return page?.Items ?? Array.Empty<UserDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            _positionOptions = await EnumOptionsService.GetOptionsAsync("StaffPosition"),
            Snackbar,
            () => T.StaffDialog.Unreachable);

        if (Staff is null)
            return;

        _model.FullName = Staff.FullName;
        _model.PhoneNumber = Staff.PhoneNumber;
        _model.Position = Staff.PositionKey;
        _model.HireDate = Staff.HireDate;
        _model.Department = Staff.Department;
        _model.Specialization = Staff.Specialization;
        _model.LicenseNumber = Staff.LicenseNumber;
        _model.SalaryCalculationRule = Staff.SalaryCalculationRule;

        try
        {
            _selectedUser = await IdentityService.GetUserByIdAsync(Staff.UserId);
        }
        catch (Exception)
        {
            _selectedUser = new UserDto(
                Staff.UserId,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                true,
                false,
                false,
                false,
                null,
                null,
                default);
        }
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        if (_selectedUser is null)
        {
            Snackbar.Add(T.StaffDialog.SelectUser, Severity.Warning);
            return;
        }

        var input = new StaffInput
        {
            UserId = _selectedUser.Id,
            FullName = _model.FullName,
            PhoneNumber = _model.PhoneNumber,
            Position = _model.Position,
            HireDate = _model.HireDate ?? DateTime.Today,
            Department = _model.Department,
            Specialization = _model.Specialization,
            LicenseNumber = _model.LicenseNumber,
            SalaryCalculationRule = _model.SalaryCalculationRule,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? ResourceService.UpdateStaffAsync(Staff!.Id, input)
                    : ResourceService.CreateStaffAsync(input),
                () => T.StaffDialog.Unreachable))
        {
            Snackbar.Add(IsEdit ? T.StaffDialog.Updated : T.StaffDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }
}
