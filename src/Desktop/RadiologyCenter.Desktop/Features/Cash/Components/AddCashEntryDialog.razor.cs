using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RadiologyCenter.Desktop.Features.Cash.Models;
using RadiologyCenter.Desktop.Features.Cash.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Features.Cash.Components;

public partial class AddCashEntryDialog : EditorDialogBase
{
    [Parameter] public string SessionId { get; set; } = string.Empty;

    private IReadOnlyList<EnumOptionDto> _directionOptions = Array.Empty<EnumOptionDto>();
    private IReadOnlyList<EnumOptionDto> _reasonOptions = Array.Empty<EnumOptionDto>();

    private readonly AddEntryFormModel _model = new();
    private EditContext _editContext = default!;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            {
                _directionOptions = await EnumOptionsService.GetOptionsAsync("CashEntryDirection");
                _reasonOptions = await EnumOptionsService.GetOptionsAsync("CashEntryReason");
            },
            Snackbar,
            () => T.CashEntry.UnreachableRetry);

        _model.Reason = "Payment";
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var input = new AddCashEntryInput
        {
            CashSessionId = SessionId,
            Direction = _model.Direction,
            Reason = _model.Reason,
            Amount = _model.Amount,
            Description = _model.Description,
            ReferenceId = _model.ReferenceId,
        };

        if (await TrySaveAsync(
                () => CashService.AddEntryAsync(SessionId, input),
                () => T.CashEntry.UnreachableRetry))
        {
            Snackbar.Add(T.CashEntry.Added, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }


}
