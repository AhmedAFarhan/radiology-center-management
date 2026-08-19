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

namespace RadiologyCenter.Desktop.Components.Pages.Dashboard;

public partial class Dashboard : ComponentBase
{
private DashboardData? _data;
    private InsuranceStatsDto? _insuranceStats;
    private bool _loading = true;
    private bool _error;
    private string? _errorDetail;

    protected override async Task OnInitializedAsync()
    {
        await Task.WhenAll(LoadAsync(), LoadInsuranceAsync());
    }

    private async Task LoadAsync()
    {
        _loading = true;
        _error = false;
        try
        {
            _data = await DashboardService.LoadAsync();
        }
        catch (Exception ex)
        {
            _error = true;
            _errorDetail = ex.Message;
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task LoadInsuranceAsync()
    {
        try
        {
            _insuranceStats = await InsuranceService.GetStatsAsync();
        }
        catch
        {
            _insuranceStats = null;
        }
    }
}