using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using MudBlazor;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Services;

namespace RadiologyCenter.Desktop.Components;

/// <summary>
/// Shared plumbing for the server-side-paged list pages. Derived pages supply
/// the typed page loader and the "server unreachable" message; the base
/// provides the MudTable state, the 3-catch load path, the debounced search,
/// reload helpers and the standard editor-dialog options.
/// </summary>
public abstract class ListPageBase<TItem> : ComponentBase, IDisposable
{
    private CancellationTokenSource? _searchCts;

    [Inject] protected IDialogService DialogService { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected AppLocalizer T { get; set; } = default!;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    protected MudTable<TItem>? Table { get; set; }
    protected string? Search { get; set; }
    protected string? LoadError { get; set; }
    protected bool Offline { get; set; }

    [SupplyParameterFromQuery(Name = "q")]
    public string? SearchQuery { get; set; }

    [SupplyParameterFromQuery(Name = "open")]
    public string? OpenId { get; set; }

    private string? _openId;

    /// <summary>Loads one page of rows for the table.</summary>
    protected abstract Task<PagedResult<TItem>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct);

    /// <summary>Message shown when the server cannot be reached.</summary>
    protected abstract string UnreachableMessage { get; }

    /// <summary>Route to navigate back to after handling an "open" deep link.</summary>
    protected virtual string BaseRoute => string.Empty;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!string.IsNullOrWhiteSpace(OpenId) && Guid.TryParse(OpenId, out _))
            _openId = OpenId;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_openId is not null)
        {
            var id = _openId;
            _openId = null;
            await OpenByDeepLinkAsync(id);
        }
    }

    /// <summary>Opens the detail/editor dialog referenced by an "open" deep link.</summary>
    protected virtual Task OpenByDeepLinkAsync(string id)
        => Task.CompletedTask;

    protected async Task<TableData<TItem>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await LoadPageAsync(
                Search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            LoadError = null;
            Offline = false;
            return new TableData<TItem> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<TItem> { Items = Array.Empty<TItem>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(SafeExecute.FormatError(ex), Severity.Error);
            LoadError = ex.Message;
            Offline = false;
            return new TableData<TItem> { Items = Array.Empty<TItem>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(UnreachableMessage, Severity.Error);
            LoadError = UnreachableMessage;
            Offline = true;
            return new TableData<TItem> { Items = Array.Empty<TItem>(), TotalItems = 0 };
        }
    }

    protected async Task OnSearchChanged(string? value)
    {
        Search = value;

        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(400, cts.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (Table is not null)
            await Table.ReloadServerData();
    }

    protected Task ReloadAsync()
        => Table is null ? Task.CompletedTask : Table.ReloadServerData();

    protected async Task ReloadIfSavedAsync(IDialogReference dialog)
    {
        var result = await dialog.Result;
        if (result is { Canceled: false })
            await ReloadAsync();
    }

    protected static DialogOptions EditorDialogOptions
        => new() { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };

    public void Dispose() => _searchCts?.Cancel();
}