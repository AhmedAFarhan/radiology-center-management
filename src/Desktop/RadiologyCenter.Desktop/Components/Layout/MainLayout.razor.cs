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
using RadiologyCenter.Desktop.Components.Pages.Auth;
using FocusEventArgs = Microsoft.AspNetCore.Components.Web.FocusEventArgs;

namespace RadiologyCenter.Desktop.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
[CascadingParameter]
    private Task<AuthenticationState>? AuthStateTask { get; set; }

    private bool _drawerOpen = true;
    private string _userName = string.Empty;

    private MudTextField<string>? _searchField;
    private string _searchText = string.Empty;
    private IReadOnlyList<GlobalSearchGroupDto>? _results;
    private readonly List<SearchFlatItem> _flatItems = new();
    private CancellationTokenSource? _searchCts;
    private bool _resultsOpen;
    private bool _searching;
    private bool _searchError;
    private bool _showRecents;
    private int _selectedIndex = -1;
    private List<string> _highlightWords = new();
    private IReadOnlyList<string> _recentSearches = Array.Empty<string>();

    private sealed record SearchFlatItem(string EntityType, GlobalSearchItemDto Item);

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (AuthStateTask is not null)
            _ = RefreshUserAsync(AuthStateTask);
    }

    private async Task RefreshUserAsync(Task<AuthenticationState> task)
    {
        var state = await task;
        _userName = state.User.Identity?.Name ?? string.Empty;
    }

    private async Task OnLogout()
    {
        await AuthService.SignOutAsync();
        Snackbar.Add(T.Layout.SignedOut, Severity.Info);
        Navigation.NavigateTo("/");
    }

    private async Task OnChangePassword()
    {
        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader = true };
        await DialogService.ShowAsync<ChangePasswordDialog>(T.ChangePassword.Title, options);
    }

    private void OnNotifications()
        => Snackbar.Add(T.Layout.NoNotifications, Severity.Info);

    private string GetInitial()
    {
        if (string.IsNullOrWhiteSpace(_userName))
            return "?";
        return _userName[..1].ToUpperInvariant();
    }

    private async Task HandleGlobalKeyDown(KeyboardEventArgs e)
    {
        if ((e.CtrlKey || e.MetaKey) && string.Equals(e.Key, "k", StringComparison.OrdinalIgnoreCase))
        {
            await FocusSearchAsync();
        }
        else if (string.Equals(e.Key, "Escape", StringComparison.OrdinalIgnoreCase) && _resultsOpen)
        {
            CloseSearch();
        }
    }

    private async Task FocusSearchAsync()
    {
        if (_searchField is not null)
            await _searchField.FocusAsync();

        if (string.IsNullOrWhiteSpace(_searchText))
            ShowRecents();
    }

    private void OnSearchBoxClick()
    {
        if (string.IsNullOrWhiteSpace(_searchText) && !_searching && !_resultsOpen)
            ShowRecents();
    }

    private void ShowRecents()
    {
        _recentSearches = SearchHistory.Get(_userName);
        _results = null;
        _flatItems.Clear();
        _searching = false;
        _searchError = false;
        _selectedIndex = -1;
        _showRecents = true;
        _resultsOpen = true;
        StateHasChanged();
    }

    private async Task OnSearchTextChanged(string? value)
    {
        _searchText = value ?? string.Empty;
        _highlightWords = ParseWords(_searchText);
        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();
        _selectedIndex = -1;
        _showRecents = false;

        if (string.IsNullOrWhiteSpace(_searchText))
        {
            _results = null;
            _flatItems.Clear();
            _resultsOpen = false;
            _searching = false;
            _searchError = false;
            StateHasChanged();
            return;
        }

        _resultsOpen = true;
        StateHasChanged();

        if (_searchText.Trim().Length < 2)
        {
            _results = null;
            _flatItems.Clear();
            return;
        }

        await RunSearchAsync(cts);
    }

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

        _searching = true;
        _searchError = false;
        StateHasChanged();

        try
        {
            var groups = await SearchService.SearchAsync(_searchText.Trim(), 5, cts.Token);
            if (cts.IsCancellationRequested)
                return;
            _results = groups;
            RebuildFlatList();
        }
        catch (OperationCanceledException)
        {
            if (cts.IsCancellationRequested)
                return;
            _searchError = true;
        }
        catch (ApiException)
        {
            if (cts.IsCancellationRequested)
                return;
            _searchError = true;
        }
        catch (Exception)
        {
            if (cts.IsCancellationRequested)
                return;
            _searchError = true;
        }
        finally
        {
            if (!cts.IsCancellationRequested)
            {
                _searching = false;
                StateHasChanged();
            }
        }
    }

    private async Task RetrySearchAsync()
    {
        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();
        _searchError = false;
        _resultsOpen = true;
        StateHasChanged();
        await RunSearchAsync(cts);
    }

    private async Task RecentClickedAsync(string term)
        => await OnSearchTextChanged(term);

    private void RebuildFlatList()
    {
        _flatItems.Clear();
        _selectedIndex = -1;
        if (_results is null)
            return;

        foreach (var group in _results)
        {
            foreach (var item in group.Items)
                _flatItems.Add(new SearchFlatItem(group.EntityType, item));
        }
    }

    private async Task OnSearchKeyDown(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "ArrowDown":
                if (_showRecents && _recentSearches.Count > 0)
                {
                    _selectedIndex = 0;
                    break;
                }

                if (_flatItems.Count > 0)
                    _selectedIndex = Math.Min(_selectedIndex + 1, _flatItems.Count - 1);
                StateHasChanged();
                break;
            case "ArrowUp":
                if (_flatItems.Count > 0)
                    _selectedIndex = Math.Max(_selectedIndex - 1, 0);
                StateHasChanged();
                break;
            case "Enter":
                if (_selectedIndex >= 0 && _selectedIndex < _flatItems.Count)
                    await ActivateAsync(_flatItems[_selectedIndex]);
                else if (_showRecents && _recentSearches.Count > 0)
                    await RecentClickedAsync(_recentSearches[0]);
                break;
            case "Escape":
                CloseSearch();
                break;
        }
    }

    private void OnSearchBlur(FocusEventArgs e)
    {
        if (_resultsOpen)
            CloseSearch();
    }

    private void SelectItem(GlobalSearchItemDto item)
    {
        _selectedIndex = _flatItems.FindIndex(f => ReferenceEquals(f.Item, item));
        StateHasChanged();
    }

    private bool IsSelected(GlobalSearchItemDto item)
        => _selectedIndex >= 0 && _selectedIndex < _flatItems.Count
           && ReferenceEquals(_flatItems[_selectedIndex].Item, item);

    private async Task ActivateAsync(SearchFlatItem item)
    {
        SearchHistory.Add(_userName, _searchText.Trim());
        var route = GetRoute(item);
        CloseSearch();
        if (route is not null)
            Navigation.NavigateTo(route);
        await Task.CompletedTask;
    }

    private async Task ViewAllAsync(string entityType)
    {
        SearchHistory.Add(_userName, _searchText.Trim());
        var route = GetListRoute(entityType);
        CloseSearch();
        if (route is not null)
            Navigation.NavigateTo(route + $"?q={Uri.EscapeDataString(_searchText.Trim())}");
        await Task.CompletedTask;
    }

    private void CloseSearch()
    {
        _resultsOpen = false;
        _showRecents = false;
        _selectedIndex = -1;
        StateHasChanged();
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

    private string GroupLabel(string entityType)
        => entityType switch
        {
            "patient" => T.Search.Group.Patients,
            "staff" => T.Search.Group.Staff,
            "referralDoctor" => T.Search.Group.ReferralDoctors,
            "item" => T.Search.Group.Items,
            "supplier" => T.Search.Group.Suppliers,
            "insuranceCompany" => T.Search.Group.InsuranceCompanies,
            "insurancePolicy" => T.Search.Group.InsurancePolicies,
            "user" => T.Search.Group.Users,
            "examinationType" => T.Search.Group.ExaminationTypes,
            _ => entityType,
        };

    private static List<string> ParseWords(string text)
        => text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    private RenderFragment Highlighted(string? text)
    {
        var raw = text ?? string.Empty;
        return builder =>
        {
            if (string.IsNullOrEmpty(raw) || _highlightWords.Count == 0)
            {
                builder.AddContent(0, raw);
                return;
            }

            var spans = new List<(int Start, int Length)>();
            foreach (var word in _highlightWords)
            {
                if (string.IsNullOrWhiteSpace(word))
                    continue;

                var index = 0;
                while (index < raw.Length)
                {
                    var found = raw.IndexOf(word, index, StringComparison.OrdinalIgnoreCase);
                    if (found < 0)
                        break;
                    spans.Add((found, word.Length));
                    index = found + word.Length;
                }
            }

            if (spans.Count == 0)
            {
                builder.AddContent(0, raw);
                return;
            }

            var merged = new List<(int Start, int Length)>();
            foreach (var span in spans.OrderBy(s => s.Start).ThenByDescending(s => s.Length))
            {
                if (merged.Count == 0 || span.Start > merged[^1].Start + merged[^1].Length)
                {
                    merged.Add(span);
                }
                else
                {
                    var last = merged[^1];
                    var end = Math.Max(last.Start + last.Length, span.Start + span.Length);
                    merged[^1] = (last.Start, end - last.Start);
                }
            }

            var cursor = 0;
            var seq = 0;
            foreach (var span in merged)
            {
                if (span.Start > cursor)
                {
                    builder.AddContent(seq++, raw.Substring(cursor, span.Start - cursor));
                }

                builder.OpenElement(seq++, "mark");
                builder.AddAttribute(seq++, "class", "global-search-mark");
                builder.AddContent(seq++, raw.Substring(span.Start, span.Length));
                builder.CloseElement();

                cursor = span.Start + span.Length;
            }

            if (cursor < raw.Length)
                builder.AddContent(seq++, raw.Substring(cursor));
        };
    }
}