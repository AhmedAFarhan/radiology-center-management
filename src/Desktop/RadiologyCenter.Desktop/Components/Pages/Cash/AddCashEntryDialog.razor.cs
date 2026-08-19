using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components.Pages.Cash;

public partial class AddCashEntryDialog : EditorDialogBase
{
    [Parameter] public string SessionId { get; set; } = string.Empty;

    private readonly AddEntryFormModel _model = new();
    private EditContext _editContext = default!;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);
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