using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Features.Cash.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Cash.Pages;

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


}
