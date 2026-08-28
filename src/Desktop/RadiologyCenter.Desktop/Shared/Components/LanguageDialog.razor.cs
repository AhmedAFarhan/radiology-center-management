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

namespace RadiologyCenter.Desktop.Shared.Components;

public partial class LanguageDialog : ComponentBase
{
[CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private string _selected = AppLocalizer.DefaultCulture;
    private string _current = AppLocalizer.DefaultCulture;
    private bool _busy;

    protected override void OnInitialized()
    {
        _current = T.CurrentCulture;
        _selected = _current;
    }

    private bool IsSelected(string culture) => string.Equals(_selected, culture, StringComparison.Ordinal);

    private void Select(string culture) => _selected = culture;

    private async Task ApplyAsync()
    {
        if (_selected == _current)
        {
            MudDialog.Close(DialogResult.Ok(_selected));
            return;
        }

        _busy = true;
        try
        {
            T.SetCulture(_selected);
            MudDialog.Close(DialogResult.Ok(_selected));
        }
        finally
        {
            _busy = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();
}
