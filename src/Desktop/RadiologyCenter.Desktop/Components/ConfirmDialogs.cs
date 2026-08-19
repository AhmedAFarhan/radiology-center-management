using MudBlazor;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components;

public static class ConfirmDialogs
{
    /// <summary>
    /// Shows a <see cref="ConfirmDialog"/> for active/inactive status toggles and
    /// returns whether the user confirmed.
    /// </summary>
    public static async Task<bool> ConfirmStatusChangeAsync(
        IDialogService dialogService,
        AppLocalizer T,
        string title,
        string name,
        bool activating)
    {
        var parameters = new DialogParameters
        {
            ["Title"] = title,
            ["Message"] = T.FormatValue(T.Common.ToggleConfirm, activating ? T.Common.Activate : T.Common.Deactivate, name),
            ["Icon"] = activating ? Icons.Material.Filled.CheckCircle : Icons.Material.Filled.Block,
            ["Color"] = activating ? MudBlazor.Color.Success : MudBlazor.Color.Warning,
            ["ConfirmText"] = activating ? T.Common.Activate : T.Common.Deactivate,
            ["CancelText"] = T.Common.Cancel,
        };
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        var dialog = await dialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, options);
        var result = await dialog.Result;
        return result is { Canceled: false };
    }
}