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

public partial class CloseCashSessionDialog : ComponentBase
{
[Parameter] public string SessionId { get; set; } = string.Empty;

    [Parameter] public decimal ExpectedTotal { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly CloseSessionFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;
    private decimal _expectedTotal;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);
        _expectedTotal = ExpectedTotal;
        _model.CountedTotal = ExpectedTotal;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        await SafeExecute.RunAsync(async () =>
            {
                var input = new CloseCashSessionInput
                {
                    CashSessionId = SessionId,
                    CountedTotal = _model.CountedTotal,
                    ReceivingUserId = string.IsNullOrWhiteSpace(_model.ReceivingUserId) ? null : _model.ReceivingUserId.Trim(),
                    ReceivingOpeningFloat = _model.ReceivingOpeningFloat,
                    Notes = _model.Notes,
                };

                await CashService.CloseAsync(SessionId, input);
                Snackbar.Add(T.CloseCash.Closed, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            Snackbar,
            () => T.CloseCash.UnreachableRetry,
            busy => _busy = busy);
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class CloseSessionFormModel
    {
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Counted total must be a valid amount.")]
        public decimal CountedTotal { get; set; }

        [MaxLength(100, ErrorMessage = "Receiving user ID must be 100 characters or fewer.")]
        public string? ReceivingUserId { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Receiving opening float must be a valid amount.")]
        public decimal? ReceivingOpeningFloat { get; set; }

        [MaxLength(1000, ErrorMessage = "Notes must be 1000 characters or fewer.")]
        public string? Notes { get; set; }
    }
}