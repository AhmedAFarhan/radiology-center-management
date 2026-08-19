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

namespace RadiologyCenter.Desktop.Components.Pages.Inventory;

public partial class ItemEditorDialog : ComponentBase
{
[Parameter] public ItemDto? Item { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;

    private static readonly Dictionary<string, string> Categories = new()
    {
        ["ContrastMedia"] = "Contrast Media",
        ["Drug"] = "Drug",
        ["MedicalSupply"] = "Medical Supply",
        ["Consumable"] = "Consumable",
        ["Other"] = "Other",
    };

    private static readonly string[] Units = { "Piece", "Box", "Bottle", "Vial", "Ampoule", "Pack", "Tube", "Roll", "Sheet", "Kit" };

    private readonly ItemFormModel _model = new();
    private EditContext _editContext = default!;
    private bool _busy;

    private bool IsEdit => Item is not null;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);

        if (Item is null)
            return;

        _model.Name = Item.Name;
        _model.Brand = Item.Brand;
        _model.Category = Item.Category;
        _model.Unit = Item.Unit;
        _model.ReorderLevel = Item.ReorderLevel;
        _model.ReorderQuantity = Item.ReorderQuantity;
        _model.LotTracked = Item.LotTracked ? "Yes" : "No";
        _model.StorageInstructions = Item.StorageInstructions;
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var input = new ItemInput
        {
            Name = _model.Name,
            Brand = _model.Brand,
            Category = _model.Category,
            Unit = _model.Unit,
            ReorderLevel = _model.ReorderLevel,
            ReorderQuantity = _model.ReorderQuantity,
            LotTracked = string.Equals(_model.LotTracked, "Yes", StringComparison.OrdinalIgnoreCase),
            StorageInstructions = _model.StorageInstructions,
        };

        if (await SafeExecute.RunAsync(
                () => IsEdit
                    ? InventoryService.UpdateItemAsync(Item!.Id, input)
                    : InventoryService.CreateItemAsync(input),
                Snackbar,
                () => T.ItemDialog.Unreachable,
                busy => _busy = busy))
        {
            Snackbar.Add(IsEdit ? T.ItemDialog.ItemUpdated : T.ItemDialog.ItemCreated, Severity.Success);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private void CancelAsync()
        => MudDialog.Cancel();

    private sealed class ItemFormModel : IValidatableObject
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit is required.")]
        public string Unit { get; set; } = string.Empty;

        public string? Brand { get; set; }
        public int ReorderLevel { get; set; }
        public int ReorderQuantity { get; set; }
        public string LotTracked { get; set; } = "No";
        public string? StorageInstructions { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReorderLevel < 0)
                yield return new ValidationResult("Reorder level cannot be negative.", new[] { nameof(ReorderLevel) });

            if (ReorderQuantity < 0)
                yield return new ValidationResult("Reorder quantity cannot be negative.", new[] { nameof(ReorderQuantity) });
        }
    }
}