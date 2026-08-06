using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetStaffMachineAnalytics;

public static class GetStaffMachineAnalyticsQueryHandler
{
    public static async Task<Result<StaffMachineAnalyticsDto>> HandleAsync(
        GetStaffMachineAnalyticsQuery query,
        IExaminationHistoryRepository historyRepository,
        IAncillaryDirectory ancillaryDirectory,
        CancellationToken ct)
    {
        var histories = await historyRepository.GetByCompletedRangeAsync(query.From, query.To, ct);

        var staffIds = histories
            .SelectMany(h => new[] { h.RadiologistId, h.TechnicianId })
            .Distinct()
            .ToList();
        var referralIds = histories
            .Where(h => h.ReferralDoctorId is not null)
            .Select(h => h.ReferralDoctorId!.Value)
            .Distinct()
            .ToList();

        var staffNames = await ancillaryDirectory.ResolveStaffNamesAsync(staffIds, ct);
        var referralNames = await ancillaryDirectory.ResolveReferralNamesAsync(referralIds, ct);
        var machines = await ancillaryDirectory.GetActiveMachineCountByModalityAsync(ct);

        var radiologists = BuildStaffPerformance(
            histories,
            h => h.RadiologistId,
            h => h.RadiologistFee,
            staffNames);

        var technicians = BuildStaffPerformance(
            histories,
            h => h.TechnicianId,
            h => h.TechnicianFee,
            staffNames);

        var referralDoctors = histories
            .Where(h => h.ReferralDoctorId is not null)
            .GroupBy(h => h.ReferralDoctorId!.Value)
            .Select(g => new ReferralDoctorPerformanceDto(
                g.Key,
                referralNames.TryGetValue(g.Key, out var name) ? name : string.Empty,
                g.Count(),
                g.Sum(h => h.ReferralFee ?? 0m)))
            .OrderByDescending(d => d.ReferredExams)
            .ToList();

        var modalityUtilization = histories
            .GroupBy(h => h.TypeModality.Name)
            .Select(g => new ModalityUtilizationDto(
                g.Key,
                g.Count(),
                machines.TryGetValue(g.Key, out var count) ? count : 0,
                machines.TryGetValue(g.Key, out var active) && active > 0
                    ? Math.Round((decimal)g.Count() / active, 2)
                    : 0m))
            .OrderByDescending(d => d.CompletedExams)
            .ToList();

        return Result.Success(new StaffMachineAnalyticsDto(
            radiologists,
            technicians,
            referralDoctors,
            modalityUtilization));
    }

    private static IReadOnlyList<StaffPerformanceDto> BuildStaffPerformance(
        IReadOnlyList<Domain.Entities.ExaminationHistory> histories,
        Func<Domain.Entities.ExaminationHistory, Guid> selector,
        Func<Domain.Entities.ExaminationHistory, decimal?> feeSelector,
        IReadOnlyDictionary<Guid, string> names) =>
        histories
            .GroupBy(selector)
            .Select(g => new StaffPerformanceDto(
                g.Key,
                names.TryGetValue(g.Key, out var name) ? name : string.Empty,
                g.Count(),
                g.Sum(feeSelector) ?? 0m))
            .OrderByDescending(d => d.CompletedExams)
            .ToList();
}
