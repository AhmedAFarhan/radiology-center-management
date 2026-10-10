using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Inventory.Application.Commands.ActivateItem;
using RadiologyCenter.Inventory.Application.Commands.CreateItem;
using RadiologyCenter.Inventory.Application.Commands.DeactivateItem;
using RadiologyCenter.Inventory.Application.Commands.DeleteItem;
using RadiologyCenter.Inventory.Application.Commands.IssueStock;
using RadiologyCenter.Inventory.Application.Commands.ImportItems;
using RadiologyCenter.Inventory.Application.Commands.UpdateItem;
using RadiologyCenter.Inventory.Application.DTOs;
using RadiologyCenter.Inventory.Application.Queries.ExportItems;
using RadiologyCenter.Inventory.Application.Queries.GetItemById;
using RadiologyCenter.Inventory.Application.Queries.GetItems;
using RadiologyCenter.Inventory.Application.Queries.GetItemStock;
using RadiologyCenter.Inventory.Application.Queries.GetItemsImportTemplate;

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Inventory;

public class ItemsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public ItemsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(InventoryItemsReadCode)]
    [ClinicOperation("GET", "inventory/items/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemDto>>(new GetItemByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryItemsReadCode)]
    [ClinicOperation("POST", "inventory/items/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ItemDto>>>(new GetItemsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryItemsReadCode)]
    [ClinicOperation("POST", "inventory/items/export")]
    public async Task<ClinicResult> ExportAsync([FromBody] QueryRequest request, CancellationToken ct, [FromQuery] bool? isActive = null)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new ExportItemsQuery(request, isActive), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [ClinicPermission(InventoryItemsCreateCode)]
    [ClinicOperation("GET", "inventory/items/import-template")]
    public async Task<ClinicResult> GetImportTemplateAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new GetItemsImportTemplateQuery(), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [ClinicPermission(InventoryItemsCreateCode)]
    [ClinicOperation("POST", "inventory/items/import")]

    public async Task<ClinicResult> ImportAsync([FromForm] ImportExcelForm form, CancellationToken ct)
    {
        if (form.File is null || form.File.Length == 0)
            return BadRequest(new { Type = "File", Message = "An Excel file is required." });

        using var stream = new MemoryStream();
        await form.File.CopyToAsync(stream, ct);
        var result = await _bus.InvokeAsync<Result<ExcelImportResult>>(new ImportItemsCommand(stream.ToArray()), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryItemsCreateCode)]
    [ClinicOperation("POST", "inventory/items")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryItemsUpdateCode)]
    [ClinicOperation("PUT", "inventory/items/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ItemId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryItemsUpdateCode)]
    [ClinicOperation("POST", "inventory/items/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateItemCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryItemsUpdateCode)]
    [ClinicOperation("POST", "inventory/items/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateItemCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryItemsDeleteCode)]
    [ClinicOperation("DELETE", "inventory/items/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteItemCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryStockReadCode)]
    [ClinicOperation("GET", "inventory/items/{id:guid}/stock")]
    public async Task<ClinicResult> GetStockAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemStockDto>>(new GetItemStockQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryStockIssueCode)]
    [ClinicOperation("POST", "inventory/items/{id:guid}/issue")]
    public async Task<ClinicResult> IssueStockAsync(Guid id, [FromBody] IssueStockCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ItemId = id }, ct);
        return result.ToActionResult();
    }
}

public sealed class ImportExcelForm
{
    public IFormFile File { get; set; } = null!;
}
