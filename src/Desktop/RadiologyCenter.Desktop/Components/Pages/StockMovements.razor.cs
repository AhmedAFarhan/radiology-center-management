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

public partial class StockMovements : ComponentBase, IDisposable
{
private MudTable<StockMovementDto>? _table;
    private string? _search;
    private CancellationTokenSource? _searchCts;
    private string? _loadError;
    private bool _offline;

    private async Task<TableData<StockMovementDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        try
        {
            var page = await InventoryService.GetStockMovementsPagedAsync(
                _search,
                state.SortLabel,
                state.SortDirection == SortDirection.Descending,
                state.Page + 1,
                state.PageSize,
                ct);

            _loadError = null;
            _offline = false;
            return new TableData<StockMovementDto> { Items = page.Items, TotalItems = page.TotalCount };
        }
        catch (OperationCanceledException)
        {
            if (ct.IsCancellationRequested)
                return new TableData<StockMovementDto> { Items = Array.Empty<StockMovementDto>(), TotalItems = 0 };
            throw;
        }
        catch (ApiException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            _loadError = ex.Message;
            _offline = false;
            return new TableData<StockMovementDto> { Items = Array.Empty<StockMovementDto>(), TotalItems = 0 };
        }
        catch (Exception)
        {
            Snackbar.Add(T.StockMovements.Unreachable, Severity.Error);
            _loadError = T.StockMovements.Unreachable;
            _offline = true;
            return new TableData<StockMovementDto> { Items = Array.Empty<StockMovementDto>(), TotalItems = 0 };
        }
    }

    private async Task OnSearchChanged(string? value)
    {
        _search = value;

        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(400, cts.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        if (_table is not null)
            await _table.ReloadServerData();
    }

    private Task ReloadAsync()
        => _table is null ? Task.CompletedTask : _table.ReloadServerData();

    private static string FormatMovementType(string type) => type switch
    {
        "ReturnToSupplier" => "Return to Supplier",
        _ => type,
    };

    private static string ShortId(string id)
        => id.Length > 8 ? id[..8] : id;

    public void Dispose() => _searchCts?.Cancel();
}