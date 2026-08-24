using Microsoft.AspNetCore.Mvc;
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
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Inventory;

[ApiController]
[Route("api/inventory/items")]
public class ItemsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ItemsController(IMessageBus bus) => _bus = bus;

    [HasPermission(InventoryItemsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemDto>>(new GetItemByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ItemDto>>>(new GetItemsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsReadCode)]
    [HttpPost("export")]
    public async Task<IActionResult> ExportAsync([FromBody] QueryRequest request, CancellationToken ct, [FromQuery] bool? isActive = null)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new ExportItemsQuery(request, isActive), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HasPermission(InventoryItemsCreateCode)]
    [HttpGet("import-template")]
    public async Task<IActionResult> GetImportTemplateAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<FileContentDto>>(new GetItemsImportTemplateQuery(), ct);
        if (!result.IsSuccess)
            return result.ToActionResult();
        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HasPermission(InventoryItemsCreateCode)]
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportAsync([FromForm] ImportExcelForm form, CancellationToken ct)
    {
        if (form.File is null || form.File.Length == 0)
            return BadRequest(new { Type = "File", Message = "An Excel file is required." });

        using var stream = new MemoryStream();
        await form.File.CopyToAsync(stream, ct);
        var result = await _bus.InvokeAsync<Result<ExcelImportResult>>(new ImportItemsCommand(stream.ToArray()), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ItemId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsUpdateCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateItemCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsUpdateCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateItemCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsDeleteCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteItemCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryStockReadCode)]
    [HttpGet("{id:guid}/stock")]
    public async Task<IActionResult> GetStockAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemStockDto>>(new GetItemStockQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryStockIssueCode)]
    [HttpPost("{id:guid}/issue")]
    public async Task<IActionResult> IssueStockAsync(Guid id, [FromBody] IssueStockCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ItemId = id }, ct);
        return result.ToActionResult();
    }
}

public sealed class ImportExcelForm
{
    public IFormFile File { get; set; } = null!;
}
