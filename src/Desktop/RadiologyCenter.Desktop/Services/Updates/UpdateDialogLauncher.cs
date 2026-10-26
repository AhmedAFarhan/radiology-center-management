using MudBlazor;
using RadiologyCenter.Desktop.Shared.Components;

namespace RadiologyCenter.Desktop.Services.Updates;

public sealed class UpdateDialogLauncher(IDialogService dialogs)
{
    private bool _showing;
    public async Task ShowAsync()
    {
        if (_showing) return;
        _showing = true;
        try
        {
            var dialog = await dialogs.ShowAsync<ClinicUpdateDialog>("EGcare", new DialogOptions
            { MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false, CloseOnEscapeKey = false });
            await dialog.Result;
        }
        finally { _showing = false; }
    }
}
