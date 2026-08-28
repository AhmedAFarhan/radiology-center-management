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

namespace RadiologyCenter.Desktop.Features.Resources.Components;

public partial class EquipmentEditorDialog : EditorDialogBase
{
    [Parameter] public EquipmentDto? Equipment { get; set; }

    private IReadOnlyList<EnumOptionDto> _modalityOptions = Array.Empty<EnumOptionDto>();

    private readonly EquipmentFormModel _model = new();
    private EditContext _editContext = default!;

    private bool IsEdit => Equipment is not null;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
                _modalityOptions = await EnumOptionsService.GetOptionsAsync("EquipmentModality"),
            Snackbar,
            () => T.EquipmentDialog.Unreachable);

        if (Equipment is null)
            return;

        _model.Name = Equipment.Name;
        _model.Modality = Equipment.ModalityKey;
        _model.SerialNumber = Equipment.SerialNumber;
        _model.PurchaseDate = Equipment.PurchaseDate;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var input = new EquipmentInput
        {
            Name = _model.Name,
            Modality = _model.Modality,
            SerialNumber = _model.SerialNumber,
            PurchaseDate = _model.PurchaseDate,
        };

        if (await TrySaveAsync(
                () => IsEdit
                    ? ResourceService.UpdateEquipmentAsync(Equipment!.Id, input)
                    : ResourceService.CreateEquipmentAsync(input),
                () => T.EquipmentDialog.Unreachable))
        {
            Snackbar.Add(IsEdit ? T.EquipmentDialog.Updated : T.EquipmentDialog.Created, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }
}
