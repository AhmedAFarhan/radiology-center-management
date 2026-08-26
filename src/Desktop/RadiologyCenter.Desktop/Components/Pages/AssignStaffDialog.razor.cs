using Microsoft.AspNetCore.Components;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages;

public partial class AssignStaffDialog : ComponentBase
{
    [Parameter] public string ExaminationId { get; set; } = string.Empty;
    [Parameter] public string? CurrentRadiologistId { get; set; }
    [Parameter] public string? CurrentTechnicianId { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    [Inject] private ResourceService ResourceService { get; set; } = default!;
    [Inject] private ExaminationService ExaminationService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private AppLocalizer T { get; set; } = default!;

    private IReadOnlyList<StaffDto> _radiologists = Array.Empty<StaffDto>();
    private IReadOnlyList<StaffDto> _technicians = Array.Empty<StaffDto>();
    private string? _radiologistId;
    private string? _technicianId;
    private bool _loading = true;
    private bool _saving;
    private bool _busy;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var staffPage = await ResourceService.GetStaffsPagedAsync(null, null, false, 1, 200);
            _radiologists = staffPage.Items
                .Where(s => s.IsActive && s.PositionKey == "Radiologist")
                .ToList();
            _technicians = staffPage.Items
                .Where(s => s.IsActive && s.PositionKey == "Technician")
                .ToList();

            _radiologistId = CurrentRadiologistId;
            _technicianId = CurrentTechnicianId;
        }
        catch
        {
            Snackbar.Add(T.AssignStaffDialog.LoadError, Severity.Error);
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task SubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(_radiologistId))
        {
            Snackbar.Add(T.AssignStaffDialog.RadiologistRequired, Severity.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(_technicianId))
        {
            Snackbar.Add(T.AssignStaffDialog.TechnicianRequired, Severity.Warning);
            return;
        }

        _saving = true;
        _busy = true;
        StateHasChanged();

        try
        {
            await ExaminationService.AssignStaffAsync(ExaminationId, _radiologistId, _technicianId);

            Snackbar.Add(T.AssignStaffDialog.Updated, Severity.Success);
            MudDialog.Close(DialogResult.Ok(new { RadiologistId = _radiologistId, TechnicianId = _technicianId }));
        }
        catch
        {
            Snackbar.Add(T.AssignStaffDialog.UpdateError, Severity.Error);
        }
        finally
        {
            _saving = false;
            _busy = false;
        }
    }

    private void CancelAsync() => MudDialog.Cancel();
}
