
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

    [Parameter] public RenderFragment? ActionContent { get; set; }

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
