using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Domain.Entities;

namespace RadiologyCenter.Examinations.Application.Queries.GetStaffMachineAnalytics;

public static class GetStaffMachineAnalyticsQueryHandler
{
    public static async Task<Result<StaffMachineAnalyticsDto>> HandleAsync(
        GetStaffMachineAnalyticsQuery query,
        IExaminationRepository examinationRepository,
        IExaminationTypeDirectory examinationTypeDirectory,
        IAncillaryDirectory ancillaryDirectory,
        CancellationToken ct)
    {
        var examinations = await examinationRepository.GetCompletedByRangeAsync(query.From, query.To, ct);

        var typeIds = examinations.Select(e => e.ExaminationTypeId).Distinct().ToList();
        var types = await examinationTypeDirectory.GetWithItemsByIdsAsync(typeIds, ct);
        var typeLookup = types.ToDictionary(t => t.Id, t => (Name: t.Name, Modality: t.Modality));

        var staffIds = examinations
            .SelectMany(e => new[] { e.RadiologistId, e.TechnicianId })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();
        var referralIds = examinations
            .Where(e => e.ReferralDoctorId is not null)
            .Select(e => e.ReferralDoctorId!.Value)
            .Distinct()
            .ToList();

        var staffNames = await ancillaryDirectory.ResolveStaffNamesAsync(staffIds, ct);
        var referralNames = await ancillaryDirectory.ResolveReferralNamesAsync(referralIds, ct);
        var machines = await ancillaryDirectory.GetActiveMachineCountByModalityAsync(ct);

        var radiologists = BuildStaffPerformance(
            examinations,
            e => e.RadiologistId!.Value,
            e => e.RadiologistFee,
            staffNames);

        var technicians = BuildStaffPerformance(
            examinations,
            e => e.TechnicianId!.Value,
            e => e.TechnicianFee,
            staffNames);

        var referralDoctors = examinations
            .Where(e => e.ReferralDoctorId is not null)
            .GroupBy(e => e.ReferralDoctorId!.Value)
            .Select(g => new ReferralDoctorPerformanceDto(
                g.Key,
                referralNames.TryGetValue(g.Key, out var name) ? name : string.Empty,
                g.Count(),
                g.Sum(e => e.ReferralFee ?? 0m)))
            .OrderByDescending(d => d.ReferredExams)
            .ToList();

        var modalityUtilization = examinations
            .GroupBy(e => typeLookup.TryGetValue(e.ExaminationTypeId, out var t) ? t.Modality : BrandConstants.UnknownModality)
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
        IReadOnlyList<Examination> examinations,
        Func<Examination, Guid> selector,
        Func<Examination, decimal?> feeSelector,
        IReadOnlyDictionary<Guid, string> names) =>
        examinations
            .GroupBy(selector)
            .Select(g => new StaffPerformanceDto(
                g.Key,
                names.TryGetValue(g.Key, out var name) ? name : string.Empty,
                g.Count(),
                g.Sum(feeSelector) ?? 0m))
            .OrderByDescending(d => d.CompletedExams)
            .ToList();
}
