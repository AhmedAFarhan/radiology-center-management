using Microsoft.AspNetCore.Components;

namespace RadiologyCenter.Desktop.Shared.Components;

public partial class DialogButtons : ComponentBase
{
    [Parameter] public bool Busy { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string CancelLabel { get; set; } = "Cancel";
    [Parameter] public string SaveLabel { get; set; } = "Save";
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback OnSave { get; set; }
}
