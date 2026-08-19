using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages.Cash;

public partial class CloseCashSessionDialog : EditorDialogBase
{
    [Parameter] public string SessionId { get; set; } = string.Empty;

    [Parameter] public decimal ExpectedTotal { get; set; }

    private readonly CloseSessionFormModel _model = new();
    private EditContext _editContext = default!;
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

        var input = new CloseCashSessionInput
        {
            CashSessionId = SessionId,
            CountedTotal = _model.CountedTotal,
            ReceivingUserId = string.IsNullOrWhiteSpace(_model.ReceivingUserId) ? null : _model.ReceivingUserId.Trim(),
            ReceivingOpeningFloat = _model.ReceivingOpeningFloat,
            Notes = _model.Notes,
        };

        if (await TrySaveAsync(
                () => CashService.CloseAsync(SessionId, input),
                () => T.CloseCash.UnreachableRetry))
        {
            Snackbar.Add(T.CloseCash.Closed, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

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