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

namespace RadiologyCenter.Desktop.Components.Pages.Cash;

public partial class OpenCashSessionDialog : ComponentBase
{
[CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly OpenSessionFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;

    protected override void OnInitialized()
        => _editContext = new EditContext(_model);

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                var input = new OpenCashSessionInput
                {
                    OpeningFloat = _model.OpeningFloat,
                    Notes = _model.Notes,
                };

                await CashService.OpenAsync(input);
                Snackbar.Add(T.OpenCash.Opened, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.OpenCash.UnreachableRetry,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class OpenSessionFormModel
    {
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Opening float must be a valid amount.")]
        public decimal OpeningFloat { get; set; }

        [MaxLength(500, ErrorMessage = "Notes must be 500 characters or fewer.")]
        public string? Notes { get; set; }
    }
}