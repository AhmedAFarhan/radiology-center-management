using Microsoft.AspNetCore.Components;
using MudBlazor;
using Color = MudBlazor.Color;

namespace RadiologyCenter.Desktop.Components;

public partial class ConfirmDialog : ComponentBase
{
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Message { get; set; } = string.Empty;
    [Parameter] public string Icon { get; set; } = Icons.Material.Filled.HelpOutline;
    [Parameter] public Color Color { get; set; } = Color.Primary;
    [Parameter] public string ConfirmText { get; set; } = "OK";
    [Parameter] public string CancelText { get; set; } = "Cancel";

    private string IconTint => Color switch
    {
        Color.Error => "background-color: var(--mud-palette-error);",
        Color.Warning => "background-color: var(--mud-palette-warning);",
        Color.Success => "background-color: var(--mud-palette-success);",
        Color.Info => "background-color: var(--mud-palette-info);",
        _ => "background-color: var(--mud-palette-primary);",
    };

    private void OnConfirm() => MudDialog.Close(DialogResult.Ok(true));

    private void OnCancel() => MudDialog.Cancel();
}