using Microsoft.AspNetCore.Components;
using MudBlazor;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components;

/// <summary>
/// Shared plumbing for the editor/detail dialogs opened through
/// <see cref="IDialogService"/>. Provides the cascaded dialog instance, the
/// busy flag used by the footer buttons and a safe submit helper.
/// </summary>
public abstract class EditorDialogBase : ComponentBase
{
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected AppLocalizer T { get; set; } = default!;

    private bool _busy;

    protected bool Busy => _busy;

    protected void SetBusy(bool busy)
    {
        if (_busy == busy)
            return;
        _busy = busy;
        StateHasChanged();
    }

    protected void CancelAsync()
        => MudDialog.Cancel();

    /// <summary>
    /// Runs <paramref name="save"/> under <see cref="SafeExecute"/> (which shows
    /// the API/unreachable snackbar and toggles the busy flag) and returns
    /// whether the operation succeeded.
    /// </summary>
    protected async Task<bool> TrySaveAsync(Func<Task> save, Func<string> unreachable)
        => await SafeExecute.RunAsync(save, Snackbar, unreachable, SetBusy);
}