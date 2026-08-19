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

public partial class AddCashEntryDialog : ComponentBase
{
[Parameter] public string SessionId { get; set; } = string.Empty;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly AddEntryFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);
        _model.Reason = "Payment";
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                var input = new AddCashEntryInput
                {
                    CashSessionId = SessionId,
                    Direction = _model.Direction,
                    Reason = _model.Reason,
                    Amount = _model.Amount,
                    Description = _model.Description,
                    ReferenceId = _model.ReferenceId,
                };

                await CashService.AddEntryAsync(SessionId, input);
                Snackbar.Add(T.CashEntry.Added, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.CashEntry.UnreachableRetry,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class AddEntryFormModel
    {
        public string Direction { get; set; } = "In";

        public string Reason { get; set; } = "Payment";

        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [MaxLength(100, ErrorMessage = "Reference must be 100 characters or fewer.")]
        public string? ReferenceId { get; set; }

        [MaxLength(500, ErrorMessage = "Description must be 500 characters or fewer.")]
        public string? Description { get; set; }
    }
}