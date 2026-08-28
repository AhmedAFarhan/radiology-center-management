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

public partial class LeaveEditorDialog : EditorDialogBase
{
    [Parameter] public LeaveDto? Leave { get; set; }

    private IReadOnlyList<EnumOptionDto> _leaveTypeOptions = Array.Empty<EnumOptionDto>();

    private readonly LeaveFormModel _model = new();
    private EditContext _editContext = default!;
    private StaffDto? _selectedStaff;

    private bool IsEdit => Leave is not null;

    private async Task<IEnumerable<StaffDto>> SearchStaffAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<StaffDto>();

        var page = await SafeExecute.RunAsync(
            () => ResourceService.GetStaffsPagedAsync(value, "LastName", false, 1, 20, ct),
            Snackbar,
            () => T.LeaveDialog.SearchStaffError);
        return page?.Items ?? Array.Empty<StaffDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            _leaveTypeOptions = await EnumOptionsService.GetOptionsAsync("LeaveType"),
            Snackbar,
            () => T.LeaveDialog.Unreachable);

        if (Leave is null)
        {
            _model.StartDate = DateTime.Today;
            _model.EndDate = DateTime.Today;
            return;
        }

        _model.LeaveType = Leave.LeaveTypeKey;
        _model.StartDate = Leave.StartDate;
        _model.EndDate = Leave.EndDate;
        _model.Reason = Leave.Reason;

        try
        {
            _selectedStaff = await ResourceService.GetStaffByIdAsync(Leave.StaffId);
        }
        catch (Exception)
        {
            // name resolution is best-effort; ids are used as fallback on submit
        }
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        if (_selectedStaff is null)
        {
            Snackbar.Add(T.LeaveDialog.SelectStaff, Severity.Warning);
            return;
        }

        if (_model.EndDate < _model.StartDate)
        {
            Snackbar.Add(T.LeaveDialog.EndAfterStart, Severity.Warning);
            return;
        }

        var input = new LeaveInput
        {
            StaffId = _selectedStaff.Id,
            LeaveType = _model.LeaveType,
            StartDate = _model.StartDate.Value.Date,
            EndDate = _model.EndDate.Value.Date,
            Reason = _model.Reason,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? ResourceService.UpdateLeaveAsync(Leave!.Id, input)
                    : ResourceService.CreateLeaveAsync(input),
                () => T.LeaveDialog.Unreachable))
        {
            Snackbar.Add(IsEdit ? T.LeaveDialog.Updated : T.LeaveDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }
}
