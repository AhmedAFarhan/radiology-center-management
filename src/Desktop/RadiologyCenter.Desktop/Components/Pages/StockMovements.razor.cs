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

namespace RadiologyCenter.Desktop.Components.Pages;

public partial class StockMovements : ListPageBase<StockMovementDto>
{
    protected override string UnreachableMessage => T.StockMovements.Unreachable;

    protected override async Task<PagedResult<StockMovementDto>> LoadPageAsync(
        string? search,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken ct)
        => await InventoryService.GetStockMovementsPagedAsync(search, sortBy, sortDescending, page, pageSize, ct);

    private static string FormatMovementType(string type) => type switch
    {
        "ReturnToSupplier" => "Return to Supplier",
        _ => type,
    };
}