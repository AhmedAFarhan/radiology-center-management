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
using RadiologyCenter.Desktop.Features.Analytics.Models;
using RadiologyCenter.Desktop.Services;
using RadiologyCenter.Desktop.Features.Analytics.Components;

namespace RadiologyCenter.Desktop.Features.Analytics.Pages;

public partial class StaffMachineAnalytics : AnalyticsPageBase
{
[Inject]
    private AnalyticsService Api { get; set; } = null!;

    private StaffMachineAnalyticsDto? _data;

    protected override async Task LoadAsync(DateTime from, DateTime to)
    {
        _data = await Api.GetStaffMachineAsync(from, to);
    }

    private IReadOnlyList<AnalyticsSlice> BuildRadiologistSlices()
        => (_data?.Radiologists ?? Array.Empty<StaffPerformanceDto>())
            .Select(s => new AnalyticsSlice(s.Name, s.CompletedExams))
            .ToList();

    private IReadOnlyList<AnalyticsSlice> BuildTechnicianSlices()
        => (_data?.Technicians ?? Array.Empty<StaffPerformanceDto>())
            .Select(s => new AnalyticsSlice(s.Name, s.CompletedExams))
            .ToList();

    private IReadOnlyList<AnalyticsSlice> BuildReferralSlices()
        => (_data?.ReferralDoctors ?? Array.Empty<ReferralDoctorPerformanceDto>())
            .Select(r => new AnalyticsSlice(r.Name, r.ReferredExams))
            .ToList();
}

