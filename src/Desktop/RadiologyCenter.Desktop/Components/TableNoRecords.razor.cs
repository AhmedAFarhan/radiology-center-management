using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace RadiologyCenter.Desktop.Components;

public partial class TableNoRecords : ComponentBase
{
    [Parameter] public string? LoadError { get; set; }
    [Parameter] public bool Offline { get; set; }
    [Parameter] public string ErrorTitle { get; set; } = string.Empty;
    [Parameter] public string EmptyIcon { get; set; } = Icons.Material.Filled.Inbox;
    [Parameter] public string EmptyTitle { get; set; } = string.Empty;
    [Parameter] public string EmptyMessage { get; set; } = string.Empty;
    [Parameter] public string? EmptyActionLabel { get; set; }
    [Parameter] public bool ShowAction { get; set; } = true;
    [Parameter] public EventCallback OnReload { get; set; }
    [Parameter] public EventCallback OnEmptyAction { get; set; }
}