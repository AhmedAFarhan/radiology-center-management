using Microsoft.AspNetCore.Components;

namespace RadiologyCenter.Desktop.Components;

public partial class RowActions : ComponentBase
{
    [Parameter] public bool IsActive { get; set; }
    [Parameter] public bool CanEdit { get; set; } = true;
    [Parameter] public bool CanToggle { get; set; } = true;
    [Parameter] public bool CanDelete { get; set; } = true;
    [Parameter] public string EditLabel { get; set; } = "Edit";
    [Parameter] public string ToggleLabel { get; set; } = "Toggle";
    [Parameter] public string DeleteLabel { get; set; } = "Delete";
    [Parameter] public EventCallback OnEdit { get; set; }
    [Parameter] public EventCallback OnToggle { get; set; }
    [Parameter] public EventCallback OnDelete { get; set; }
}