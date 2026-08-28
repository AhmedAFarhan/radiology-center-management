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

public partial class PageHeader : ComponentBase
{
[Parameter] public string Icon { get; set; } = string.Empty;

    [Parameter] public string Title { get; set; } = string.Empty;

    [Parameter] public string Subtitle { get; set; } = string.Empty;

    [Parameter] public string? SearchPlaceholder { get; set; }

    [Parameter] public EventCallback<string?> SearchValueChanged { get; set; }

    [Parameter] public string? SearchValue { get; set; }

    [Parameter] public bool ShowSearch { get; set; } = true;

    [Parameter] public Func<Task>? OnRefresh { get; set; }

    [Parameter] public Func<Task>? OnExport { get; set; }

    [Parameter] public Func<Task>? OnImport { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }

    private string? _search;
    private string _placeholder = string.Empty;
    private string? _lastSearchValue;
    private bool _pendingExternalSearch;

    protected override void OnParametersSet()
    {
        _placeholder = string.IsNullOrWhiteSpace(SearchPlaceholder)
            ? T.Common.SearchPlaceholder
            : SearchPlaceholder;

        if (SearchValue is not null && !string.Equals(SearchValue, _lastSearchValue, StringComparison.Ordinal))
        {
            _search = SearchValue;
            _lastSearchValue = SearchValue;
            _pendingExternalSearch = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_pendingExternalSearch)
        {
            _pendingExternalSearch = false;
            await SearchValueChanged.InvokeAsync(_search);
        }
    }

    private Task OnLocalSearchChanged(string value)
    {
        _search = value;
        return SearchValueChanged.InvokeAsync(value);
    }
}
