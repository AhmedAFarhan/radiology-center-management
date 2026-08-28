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

using RadiologyCenter.Desktop.Features.Examinations.Models;

namespace RadiologyCenter.Desktop.Features.Examinations.Components;

public partial class ExaminationTypeEditorDialog : EditorDialogBase
{
[Parameter] public ExaminationTypeDto? Type { get; set; }

    private IReadOnlyList<EnumOptionDto> _modalityOptions = Array.Empty<EnumOptionDto>();

    private readonly TypeFormModel _model = new();
    private EditContext _editContext = default!;
    private List<ItemDto> _inventoryItems = new();

    private bool IsEdit => Type is not null;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_model);

        await SafeExecute.RunAsync(async () =>
            {
                var items = await InventoryService.GetItemsPagedAsync(null, "Name", false, 1, 200);
                _inventoryItems = items.Items.ToList();
                _modalityOptions = await EnumOptionsService.GetOptionsAsync("Modality");
            },
            Snackbar,
            () => T.ExamDialog.LoadItemsError);

        if (Type is null)
            return;

        _model.Name = Type.Name;
        _model.Modality = Type.ModalityKey;
        _model.BodyPart = Type.BodyPart;
        _model.StandardDurationMinutes = Type.StandardDurationMinutes;
        _model.Price = Type.Price;
        _model.RequiresPreparation = Type.RequiresPreparation;
        _model.RequiresConsent = Type.RequiresConsent;
        _model.Items = Type.Items is null ? new() : Type.Items.Select(i => new TypeItemModel
        {
            ItemId = i.ItemId,
            Quantity = i.Quantity,
            IsContrast = i.IsContrast,
            IsRequired = i.IsRequired,
            Notes = i.Notes,
        }).ToList();
    }

    private void AddItem() => _model.Items.Add(new TypeItemModel { Quantity = 1 });

    private void RemoveItem(TypeItemModel item) => _model.Items.Remove(item);

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        var missingItems = _model.Items.Any(i => string.IsNullOrWhiteSpace(i.ItemId));
        var duplicateItems = _model.Items.GroupBy(i => i.ItemId).Any(g => g.Count() > 1);
        var invalidQuantities = _model.Items.Any(i => i.Quantity <= 0);

        if (missingItems || duplicateItems || invalidQuantities)
        {
            Snackbar.Add(T.ExamDialog.ItemsInvalid, Severity.Warning);
            return;
        }

        await TrySaveAsync(async () =>
            {
                var input = new ExaminationTypeInput
                {
                    Name = _model.Name,
                    Modality = _model.Modality,
                    BodyPart = _model.BodyPart,
                    StandardDurationMinutes = _model.StandardDurationMinutes,
                    Price = _model.Price,
                    RequiresPreparation = _model.RequiresPreparation,
                    RequiresConsent = _model.RequiresConsent,
                    Items = _model.Items.Select(i => new ExaminationTypeItemInput
                    {
                        ItemId = i.ItemId,
                        Quantity = i.Quantity,
                        IsContrast = i.IsContrast,
                        IsRequired = i.IsRequired,
                        Notes = i.Notes,
                    }).ToList(),
                };

                if (IsEdit)
                    await ExaminationService.UpdateTypeAsync(Type!.Id, input);
                else
                    await ExaminationService.CreateTypeAsync(input);

                Snackbar.Add(IsEdit ? T.ExamDialog.Updated : T.ExamDialog.Created, Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            },
            () => T.ExamDialog.Unreachable);
    }

}

