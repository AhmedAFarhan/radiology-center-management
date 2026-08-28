using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Cash.Pages;

public partial class OpenCashSessionDialog : EditorDialogBase
{
    private readonly OpenSessionFormModel _model = new();
    private EditContext _editContext = default!;

    protected override void OnInitialized()
        => _editContext = new EditContext(_model);

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var input = new OpenCashSessionInput
        {
            OpeningFloat = _model.OpeningFloat,
            Notes = _model.Notes,
        };

        if (await TrySaveAsync(
                () => CashService.OpenAsync(input),
                () => T.OpenCash.UnreachableRetry))
        {
            Snackbar.Add(T.OpenCash.Opened, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private sealed class OpenSessionFormModel
    {
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Opening float must be a valid amount.")]
        public decimal OpeningFloat { get; set; }

        [MaxLength(500, ErrorMessage = "Notes must be 500 characters or fewer.")]
        public string? Notes { get; set; }
    }
}
