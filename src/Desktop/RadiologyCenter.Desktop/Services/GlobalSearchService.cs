using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class GlobalSearchService : IDisposable
{
    private readonly SearchService _searchService;
    private readonly SearchHistoryService _searchHistory;
    private readonly NavigationManager _navigation;
    private CancellationTokenSource? _searchCts;

    public GlobalSearchService(
        SearchService searchService,
        SearchHistoryService searchHistory,
        NavigationManager navigation)
    {
        _searchService = searchService;
        _searchHistory = searchHistory;
        _navigation = navigation;
    }

    // ── State ──

    public string SearchText { get; set; } = string.Empty;
    public IReadOnlyList<GlobalSearchGroupDto>? Results { get; set; }
    public List<SearchFlatItem> FlatItems { get; } = new();
    public bool ResultsOpen { get; set; }
    public bool Searching { get; set; }
    public bool SearchError { get; set; }
    public bool ShowRecents { get; set; }
    public int SelectedIndex { get; set; } = -1;
    public List<string> HighlightWords { get; set; } = new();
    public IReadOnlyList<string> RecentSearches { get; set; } = Array.Empty<string>();
    public string UserName { get; set; } = string.Empty;

    // ── Events ──

    public event Func<Task>? OnStateChanged;

    private Task NotifyStateChanged() => OnStateChanged?.Invoke() ?? Task.CompletedTask;

    // ── Public methods ──

    public void OnSearchBoxClick()
    {
        if (string.IsNullOrWhiteSpace(SearchText) && !Searching && !ResultsOpen)
            ShowRecentSearches();
    }

    public async Task OnSearchTextChanged(string? value)
    {
        SearchText = value ?? string.Empty;
        HighlightWords = ParseWords(SearchText);
        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();
        SelectedIndex = -1;
        ShowRecents = false;

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Results = null;
            FlatItems.Clear();
            ResultsOpen = false;
            Searching = false;
            SearchError = false;
            await NotifyStateChanged();
            return;
        }

        ResultsOpen = true;
        await NotifyStateChanged();

        if (SearchText.Trim().Length < 2)
        {
            Results = null;
            FlatItems.Clear();
            return;
        }

        await RunSearchAsync(cts);
    }

    public async Task OnSearchKeyDown(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "ArrowDown":
                if (ShowRecents && RecentSearches.Count > 0)
                {
                    SelectedIndex = 0;
                    break;
                }

                if (FlatItems.Count > 0)
                    SelectedIndex = Math.Min(SelectedIndex + 1, FlatItems.Count - 1);
                await NotifyStateChanged();
                break;
            case "ArrowUp":
                if (FlatItems.Count > 0)
                    SelectedIndex = Math.Max(SelectedIndex - 1, 0);
                await NotifyStateChanged();
                break;
            case "Enter":
                if (SelectedIndex >= 0 && SelectedIndex < FlatItems.Count)
                    await OnActivateItem(FlatItems[SelectedIndex]);
                else if (ShowRecents && RecentSearches.Count > 0)
                    await OnRecentClicked(RecentSearches[0]);
                break;
            case "Escape":
                CloseSearch();
                break;
        }
    }

    public void OnSearchBlur(Microsoft.AspNetCore.Components.Web.FocusEventArgs e)
    {
        if (ResultsOpen)
            CloseSearch();
    }

    public void CloseSearch()
    {
        ResultsOpen = false;
        ShowRecents = false;
        SelectedIndex = -1;
        _ = NotifyStateChanged();
    }

    public void ShowRecentSearches()
    {
        RecentSearches = _searchHistory.Get(UserName);
        Results = null;
        FlatItems.Clear();
        Searching = false;
        SearchError = false;
        SelectedIndex = -1;
        ShowRecents = true;
        ResultsOpen = true;
        _ = NotifyStateChanged();
    }

    public async Task OnRecentClicked(string term)
        => await OnSearchTextChanged(term);

    public async Task RetrySearchAsync()
    {
        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();
        SearchError = false;
        ResultsOpen = true;
        await NotifyStateChanged();
        await RunSearchAsync(cts);
    }

    public async Task OnActivateItem(SearchFlatItem item)
    {
        _searchHistory.Add(UserName, SearchText.Trim());
        var route = GetRoute(item);
        CloseSearch();
        if (route is not null)
            _navigation.NavigateTo(route);
        await Task.CompletedTask;
    }

    public async Task OnViewAll(string entityType)
    {
        _searchHistory.Add(UserName, SearchText.Trim());
        var route = GetListRoute(entityType);
        CloseSearch();
        if (route is not null)
            _navigation.NavigateTo(route + $"?q={Uri.EscapeDataString(SearchText.Trim())}");
        await Task.CompletedTask;
    }

    // ── Private helpers ──

    private async Task RunSearchAsync(CancellationTokenSource cts)
    {
        try
        {
            await Task.Delay(250, cts.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (cts.IsCancellationRequested)
            return;

        Searching = true;
        SearchError = false;
        await NotifyStateChanged();

        try
        {
            var groups = await _searchService.SearchAsync(SearchText.Trim(), 5, cts.Token);
            if (cts.IsCancellationRequested)
                return;
            Results = groups;
            RebuildFlatList();
        }
        catch (OperationCanceledException)
        {
            if (cts.IsCancellationRequested)
                return;
            SearchError = true;
        }
        catch (ApiException)
        {
            if (cts.IsCancellationRequested)
                return;
            SearchError = true;
        }
        catch (Exception)
        {
            if (cts.IsCancellationRequested)
                return;
            SearchError = true;
        }
        finally
        {
            if (!cts.IsCancellationRequested)
            {
                Searching = false;
                await NotifyStateChanged();
            }
        }
    }

    private void RebuildFlatList()
    {
        FlatItems.Clear();
        SelectedIndex = -1;
        if (Results is null)
            return;

        foreach (var group in Results)
        {
            foreach (var item in group.Items)
                FlatItems.Add(new SearchFlatItem(group.EntityType, item));
        }
    }

    private static string? GetRoute(SearchFlatItem item)
        => item.EntityType switch
        {
            "patient" => $"/patients?open={item.Item.Id}",
            "staff" => $"/resources/staff?open={item.Item.Id}",
            "referralDoctor" => $"/resources/referral-doctors?open={item.Item.Id}",
            "item" => $"/inventory/items?open={item.Item.Id}",
            "supplier" => $"/inventory/suppliers?open={item.Item.Id}",
            "insuranceCompany" => $"/insurance/companies?open={item.Item.Id}",
            "insurancePolicy" => $"/insurance/policies?open={item.Item.Id}",
            "user" => $"/users?open={item.Item.Id}",
            "examinationType" => $"/examinations?open={item.Item.Id}",
            _ => null,
        };

    private static string? GetListRoute(string entityType)
        => entityType switch
        {
            "patient" => "/patients",
            "staff" => "/resources/staff",
            "referralDoctor" => "/resources/referral-doctors",
            "item" => "/inventory/items",
            "supplier" => "/inventory/suppliers",
            "insuranceCompany" => "/insurance/companies",
            "insurancePolicy" => "/insurance/policies",
            "user" => "/users",
            "examinationType" => "/examinations",
            _ => null,
        };

    private static List<string> ParseWords(string text)
        => text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    public void Dispose()
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
    }
}

public sealed record SearchFlatItem(string EntityType, GlobalSearchItemDto Item);
