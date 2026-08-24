using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Application.Queries.GetItemsImportTemplate;

public static class GetItemsImportTemplateQueryHandler
{
    public static Result<FileContentDto> Handle(
        GetItemsImportTemplateQuery query,
        IExcelService excelService)
    {
        var categories = ItemCategory.GetAll<ItemCategory>().Select(c => c.Name).ToList();
        var units = UnitType.GetAll<UnitType>().Select(u => u.Name).ToList();

        var content = excelService.CreateTemplate(
            "Items",
            [
                ("Excel.Item.Name", "Name"),
                ("Excel.Item.Brand", "Brand"),
                ("Excel.Item.Category", "Category"),
                ("Excel.Item.Unit", "Unit"),
                ("Excel.Item.ReorderLevel", "Reorder Level"),
                ("Excel.Item.ReorderQuantity", "Reorder Quantity"),
                ("Excel.Item.LotTracked", "Lot Tracked"),
                ("Excel.Item.StorageInstructions", "Storage Instructions"),
            ],
            sampleRow: ["Ultrasound Gel", "Acme", ItemCategory.Consumable.Name, UnitType.Bottle.Name, 5, 20, "No", "Store below 25C"],
            referenceSheets:
            [
                ("Category", [.. categories]),
                ("Unit", [.. units]),
            ],
            instructions:
            [
                ("Excel.Item.Instructions.RequiredColumns", "Name, Category and Unit are required; all other columns are optional."),
                ("Excel.Instructions.OneRowPerRecord", "Each row becomes one inventory item. Do not edit or reorder the header row."),
                ("Excel.Item.Instructions.AllowedValues", "Category and Unit must exactly match one of the values listed on their sheets - dropdowns are provided in Excel."),
                ("Excel.Item.Instructions.CategoryValues", $"Allowed Category values: {string.Join(", ", categories)}"),
                ("Excel.Item.Instructions.UnitValues", $"Allowed Unit values: {string.Join(", ", units)}"),
                ("Excel.Item.Instructions.YesNoFormat", "The Lot Tracked column accepts only Yes or No."),
                ("Excel.Instructions.OptionalEmpty", "Leave optional columns empty when not applicable."),
                ("Excel.Instructions.FreshTemplate", "Always fill this downloaded template; older templates may be rejected."),
            ]);

        return Result.Success(new FileContentDto(content, "inventory-items-import-template.xlsx", ExcelContentTypes.Xlsx));
    }
}
