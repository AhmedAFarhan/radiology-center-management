using Microsoft.AspNetCore.Components;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.ReadingRoom.Components;

public partial class ReadingRoomFindingsList : ComponentBase
{
    [Parameter] public IReadOnlyList<ReportFindingDto> Findings { get; set; } = Array.Empty<ReportFindingDto>();
    [Parameter] public IReadOnlyList<EnumOptionDto> SeverityOptions { get; set; } = Array.Empty<EnumOptionDto>();
    [Parameter] public bool CanEdit { get; set; }
    [Parameter] public EventCallback<(ReportFindingDto Finding, string Severity)> OnSeverityChanged { get; set; }
    [Parameter] public EventCallback<ReportFindingDto> OnRemoveFinding { get; set; }
    [Parameter] public EventCallback<(string Region, string Description, string Severity)> OnAddFinding { get; set; }

    private string _newRegion = string.Empty;
    private string _newDescription = string.Empty;
    private string _newSeverity = "None";

    private async Task AddFinding()
    {
        await OnAddFinding.InvokeAsync((_newRegion.Trim(), _newDescription.Trim(), _newSeverity));
        _newRegion = string.Empty;
        _newDescription = string.Empty;
        _newSeverity = "None";
    }
}
