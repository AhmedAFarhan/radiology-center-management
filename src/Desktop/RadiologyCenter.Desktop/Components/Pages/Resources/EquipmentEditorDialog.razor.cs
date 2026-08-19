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

public partial class EquipmentEditorDialog : ComponentBase
{
[Parameter] public EquipmentDto? Equipment { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private static readonly Dictionary<string, string> Modalities = new()
    {
        ["XRay"] = "X-Ray",
        ["CT"] = "CT",
        ["MRI"] = "MRI",
        ["Ultrasound"] = "Ultrasound",
        ["Mammography"] = "Mammography",
        ["Fluoroscopy"] = "Fluoroscopy",
        ["DEXA"] = "DEXA",
        ["Other"] = "Other",
    };

    private readonly EquipmentFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;

    private bool IsEdit => Equipment is not null;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);

        if (Equipment is null)
            return;

        _model.Name = Equipment.Name;
        _model.Modality = Equipment.Modality;
        _model.SerialNumber = Equipment.SerialNumber;
        _model.PurchaseDate = Equipment.PurchaseDate;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                var input = new EquipmentInput
                {
                    Name = _model.Name,
                    Modality = _model.Modality,
                    SerialNumber = _model.SerialNumber,
                    PurchaseDate = _model.PurchaseDate,
                };

                if (IsEdit)
                    await ResourceService.UpdateEquipmentAsync(Equipment!.Id, input);
                else
                    await ResourceService.CreateEquipmentAsync(input);

                Snackbar.Add(IsEdit ? T.EquipmentDialog.Updated : T.EquipmentDialog.Created, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.EquipmentDialog.Unreachable,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class EquipmentFormModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(200, ErrorMessage = "Name must be 200 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Modality is required.")]
        public string Modality { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Serial number must be 100 characters or fewer.")]
        public string? SerialNumber { get; set; }

        public DateTime? PurchaseDate { get; set; }
    }
}