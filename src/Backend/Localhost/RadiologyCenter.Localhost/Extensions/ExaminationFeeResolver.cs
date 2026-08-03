using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Entities;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Localhost.Extensions;

public sealed class ExaminationFeeResolver : IExaminationFeeResolver
{
    private readonly IExaminationFeeRepository _examinationFeeRepository;
    private readonly IReferralFeeRepository _referralFeeRepository;

    public ExaminationFeeResolver(
        IExaminationFeeRepository examinationFeeRepository,
        IReferralFeeRepository referralFeeRepository)
    {
        _examinationFeeRepository = examinationFeeRepository;
        _referralFeeRepository = referralFeeRepository;
    }

    public async Task<ExaminationFeeResolution?> ResolveAsync(
        Guid examinationTypeId,
        decimal examinationTypePrice,
        Guid radiologistId,
        Guid technicianId,
        Guid? referralDoctorId,
        CancellationToken ct)
    {
        var radiologistFee = await ResolveExaminationFeeAsync(examinationTypeId, ExamFeeRole.Radiologist, examinationTypePrice, ct);
        var technicianFee = await ResolveExaminationFeeAsync(examinationTypeId, ExamFeeRole.Technician, examinationTypePrice, ct);
        var referralFee = referralDoctorId.HasValue
            ? await ResolveReferralFeeAsync(referralDoctorId.Value, examinationTypeId, examinationTypePrice, ct)
            : null;

        return new ExaminationFeeResolution(radiologistFee, technicianFee, referralFee);
    }

    private async Task<decimal?> ResolveExaminationFeeAsync(
        Guid examinationTypeId,
        ExamFeeRole role,
        decimal examinationTypePrice,
        CancellationToken ct)
    {
        var spec = new DynamicSpecification<ExaminationFee>();
        spec.AddCriteria(f => f.ExaminationTypeId == examinationTypeId);
        spec.AddCriteria(f => f.Role == role);
        spec.AddCriteria(f => f.IsActive);

        var fee = await _examinationFeeRepository.FindSingleAsync(spec, ct);
        return fee is null ? null : ComputeAmount(fee.Amount, fee.IsPercentage, examinationTypePrice);
    }

    private async Task<decimal?> ResolveReferralFeeAsync(
        Guid referralDoctorId,
        Guid examinationTypeId,
        decimal examinationTypePrice,
        CancellationToken ct)
    {
        var spec = new DynamicSpecification<ReferralFee>();
        spec.AddCriteria(f => f.ReferralDoctorId == referralDoctorId);
        spec.AddCriteria(f => f.ExaminationTypeId == examinationTypeId);
        spec.AddCriteria(f => f.IsActive);

        var fee = await _referralFeeRepository.FindSingleAsync(spec, ct);
        return fee is null ? null : ComputeAmount(fee.Amount, fee.IsPercentage, examinationTypePrice);
    }

    private static decimal ComputeAmount(decimal amount, bool isPercentage, decimal examinationTypePrice)
        => isPercentage ? examinationTypePrice * amount / 100m : amount;
}