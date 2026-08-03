using Mapster;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Commands.CreateReferralFee;

public static class CreateReferralFeeCommandHandler
{
    public static async Task<Result<ReferralFeeDto>> HandleAsync(
        CreateReferralFeeCommand command,
        IReferralFeeRepository referralFeeRepository,
        IReferralDoctorDirectory referralDoctorDirectory,
        IExaminationTypeDirectory examinationTypeDirectory,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (!await referralDoctorDirectory.ExistsAsync(command.ReferralDoctorId, ct))
            return Result.Failure<ReferralFeeDto>(Error.NotFound("ReferralDoctor", command.ReferralDoctorId));

        if (!await examinationTypeDirectory.ExistsAsync(command.ExaminationTypeId, ct))
            return Result.Failure<ReferralFeeDto>(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        var existing = await FindActiveAsync(referralFeeRepository, command.ReferralDoctorId, command.ExaminationTypeId, ct);
        foreach (var fee in existing)
        {
            fee.Deactivate();
            referralFeeRepository.Update(fee);
        }

        var referralFee = ReferralFee.Create(command.ReferralDoctorId, command.ExaminationTypeId, command.Amount, command.IsPercentage);

        await referralFeeRepository.AddAsync(referralFee, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(referralFee.Adapt<ReferralFeeDto>());
    }

    private static async Task<IReadOnlyList<ReferralFee>> FindActiveAsync(
        IReferralFeeRepository repository,
        Guid referralDoctorId,
        Guid examinationTypeId,
        CancellationToken ct)
    {
        var spec = new DynamicSpecification<ReferralFee>();
        spec.AddCriteria(f => f.ReferralDoctorId == referralDoctorId);
        spec.AddCriteria(f => f.ExaminationTypeId == examinationTypeId);
        spec.AddCriteria(f => f.IsActive);
        return await repository.FindAsync(spec, ct);
    }
}