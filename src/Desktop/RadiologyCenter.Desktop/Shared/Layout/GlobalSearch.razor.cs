using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Shared.Layout;

public partial class GlobalSearch
{
    [Parameter] public string SearchText { get; set; } = string.Empty;
    [Parameter] public bool ResultsOpen { get; set; }
    [Parameter] public bool Searching { get; set; }
    [Parameter] public bool SearchError { get; set; }
    [Parameter] public bool ShowRecents { get; set; }
    [Parameter] public IReadOnlyList<string> RecentSearches { get; set; } = Array.Empty<string>();
    [Parameter] public IReadOnlyList<GlobalSearchGroupDto>? Results { get; set; }
    [Parameter] public List<SearchFlatItem> FlatItems { get; set; } = new();
    [Parameter] public int SelectedIndex { get; set; }
    [Parameter] public List<string> HighlightWords { get; set; } = new();
    [Parameter] public string UserName { get; set; } = string.Empty;

    [Parameter] public EventCallback<string?> SearchTextChanged { get; set; }
    [Parameter] public EventCallback<KeyboardEventArgs> SearchKeyDown { get; set; }
    [Parameter] public EventCallback<Microsoft.AspNetCore.Components.Web.FocusEventArgs> SearchBlur { get; set; }
    [Parameter] public EventCallback SearchBoxClick { get; set; }
    [Parameter] public EventCallback<string> RecentClicked { get; set; }
    [Parameter] public EventCallback RetrySearch { get; set; }
    [Parameter] public EventCallback<SearchFlatItem> ActivateItem { get; set; }
    [Parameter] public EventCallback<string> ViewAll { get; set; }
    [Parameter] public EventCallback CloseSearch { get; set; }

    private MudTextField<string>? _searchField;

    public async Task FocusAsync()
    {
        if (_searchField is not null)
            await _searchField.FocusAsync();
    }

    private void HandleSearchBoxClick()
        => SearchBoxClick.InvokeAsync();

    private void HandleSearchKeyDown(KeyboardEventArgs e)
        => SearchKeyDown.InvokeAsync(e);

    private void HandleSearchBlur(Microsoft.AspNetCore.Components.Web.FocusEventArgs e)
        => SearchBlur.InvokeAsync(e);

    private void HandleCloseSearch()
        => CloseSearch.InvokeAsync();

    private void HandleRetrySearch()
        => RetrySearch.InvokeAsync();

    private void OnRecentClicked(string term)
        => RecentClicked.InvokeAsync(term);

    private void OnViewAll(string entityType)
        => ViewAll.InvokeAsync(entityType);

    private void OnActivateItem(SearchFlatItem item)
        => ActivateItem.InvokeAsync(item);

    private void SelectItem(GlobalSearchItemDto item)
    {
        var idx = FlatItems.FindIndex(f => ReferenceEquals(f.Item, item));
        SelectedIndex = idx;
        StateHasChanged();
    }

    private bool IsSelected(GlobalSearchItemDto item)
        => SelectedIndex >= 0 && SelectedIndex < FlatItems.Count
           && ReferenceEquals(FlatItems[SelectedIndex].Item, item);

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
            if (string.IsNullOrEmpty(raw) || HighlightWords.Count == 0)
            {
                builder.AddContent(0, raw);
                return;
            }

            var spans = new List<(int Start, int Length)>();
            foreach (var word in HighlightWords)
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

public sealed record SearchFlatItem(string EntityType, GlobalSearchItemDto Item);
