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
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Features.Visits.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Visits.Components;

public partial class VisitScheduleDialog : ComponentBase
{
[Parameter] public ExaminationDto Visit { get; set; } = default!;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly VisitScheduleModel _model = new()
    {
        ScheduledDate = DateTime.Today,
        ScheduledTime = TimeSpan.FromHours(9),
    };
    private EditContext _editContext = default!;
    private bool _busy;

    protected override void OnInitialized()
        => _editContext = new EditContext(_model);

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        if (_model.ScheduledDate is null || _model.ScheduledTime is null)
            return;

        var scheduledAt = _model.ScheduledDate.Value.Date + _model.ScheduledTime.Value;

        await SafeExecute.RunAsync(async () =>
            {
                await ExaminationService.ScheduleAsync(Visit.Id, scheduledAt);
                Snackbar.Add(T.VisitSchedule.Scheduled, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.VisitSchedule.Unreachable,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();
}
