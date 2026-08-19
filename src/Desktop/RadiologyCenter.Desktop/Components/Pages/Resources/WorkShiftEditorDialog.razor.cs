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
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages.Resources;

public partial class WorkShiftEditorDialog : EditorDialogBase
{
    [Parameter] public WorkShiftDto? Shift { get; set; }

    private readonly WorkShiftFormModel _model = new();
    private EditContext _editContext = default!;
    private StaffDto? _selectedStaff;
    private EquipmentDto? _selectedEquipment;

    private bool IsEdit => Shift is not null;

    private async Task<IEnumerable<StaffDto>> SearchStaffAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<StaffDto>();

        try
        {
            var page = await ResourceService.GetStaffsPagedAsync(value, "LastName", false, 1, 20, ct);
            return page.Items;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            return Array.Empty<StaffDto>();
        }
        catch (Exception)
        {
            Snackbar.Add(T.WorkShiftDialog.SearchStaffError, Severity.Error);
            return Array.Empty<StaffDto>();
        }
    }

    private async Task<IEnumerable<EquipmentDto>> SearchEquipmentAsync(string? value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<EquipmentDto>();

        try
        {
            var page = await ResourceService.GetEquipmentPagedAsync(value, "Name", false, 1, 20, ct);
            return page.Items;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            return Array.Empty<EquipmentDto>();
        }
        catch (Exception)
        {
            Snackbar.Add(T.WorkShiftDialog.SearchEquipmentError, Severity.Error);
            return Array.Empty<EquipmentDto>();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        if (Shift is null)
        {
            _model.Date = DateTime.Today;
            return;
        }

        _model.Date = Shift.Date;
        _model.StartTime = Shift.StartTime;
        _model.EndTime = Shift.EndTime;
        _model.Notes = Shift.Notes;

        try
        {
            _selectedStaff = await ResourceService.GetStaffByIdAsync(Shift.StaffId);
            if (!string.IsNullOrWhiteSpace(Shift.EquipmentId))
                _selectedEquipment = await ResourceService.GetEquipmentByIdAsync(Shift.EquipmentId);
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
            Snackbar.Add(T.WorkShiftDialog.SelectStaff, Severity.Warning);
            return;
        }

        if (_model.StartTime is null || _model.EndTime is null)
        {
            Snackbar.Add(T.WorkShiftDialog.SelectTimes, Severity.Warning);
            return;
        }

        if (_model.EndTime <= _model.StartTime)
        {
            Snackbar.Add(T.WorkShiftDialog.EndAfterStart, Severity.Warning);
            return;
        }

        if (_model.Date is null)
        {
            Snackbar.Add(T.WorkShiftDialog.SelectDate, Severity.Warning);
            return;
        }

        var input = new WorkShiftInput
        {
            StaffId = _selectedStaff.Id,
            EquipmentId = _selectedEquipment?.Id,
            Date = _model.Date.Value.Date,
            StartTime = _model.StartTime.Value,
            EndTime = _model.EndTime.Value,
            Notes = _model.Notes,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? ResourceService.UpdateWorkShiftAsync(Shift!.Id, input)
                    : ResourceService.CreateWorkShiftAsync(input),
                () => T.WorkShiftDialog.Unreachable))
        {
            Snackbar.Add(IsEdit ? T.WorkShiftDialog.Updated : T.WorkShiftDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private sealed class WorkShiftFormModel
    {
        public DateTime? Date { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [MaxLength(500, ErrorMessage = "Notes must be 500 characters or fewer.")]
        public string? Notes { get; set; }
    }
}