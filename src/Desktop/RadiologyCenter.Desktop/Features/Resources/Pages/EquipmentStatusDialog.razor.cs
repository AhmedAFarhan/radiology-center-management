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

public partial class EquipmentStatusDialog : ComponentBase
{
[Parameter] public EquipmentDto Equipment { get; set; } = default!;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private IReadOnlyList<EnumOptionDto> _statusOptions = Array.Empty<EnumOptionDto>();

    private readonly EquipmentStatusModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            _statusOptions = await EnumOptionsService.GetOptionsAsync("EquipmentStatus"),
            Snackbar,
            () => T.EquipmentDialog.Unreachable);

        _model.Status = Equipment.StatusKey;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                await ResourceService.SetEquipmentStatusAsync(Equipment.Id, _model.Status);
                Snackbar.Add(T.EquipmentDialog.StatusUpdated, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.EquipmentDialog.Unreachable,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();
}
