using Microsoft.AspNetCore.Components;

namespace RadiologyCenter.Desktop.Shared.Components;

public partial class ActiveStatusChip : ComponentBase
{
    [Parameter] public bool IsActive { get; set; }
    [Parameter] public string ActiveLabel { get; set; } = "Active";
    [Parameter] public string InactiveLabel { get; set; } = "Inactive";
}
