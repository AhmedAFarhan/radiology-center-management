using Mapster;
using RadiologyCenter.Payroll.Application.Localization;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Application.Commands.CreateExaminationFee;

public static class CreateExaminationFeeCommandHandler
{
    public static async Task<Result<ExaminationFeeDto>> HandleAsync(
        CreateExaminationFeeCommand command,
        IExaminationFeeRepository examinationFeeRepository,
        IExaminationTypeDirectory examinationTypeDirectory,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (!await examinationTypeDirectory.ExistsAsync(command.ExaminationTypeId, ct))
            return Result.Failure<ExaminationFeeDto>(Error.NotFound(ErrorCodes.ExaminationTypeNotFound, "ExaminationType", command.ExaminationTypeId));

        var role = ExamFeeRole.FromName<ExamFeeRole>(command.Role);

        var existing = await FindActiveAsync(examinationFeeRepository, command.ExaminationTypeId, role, ct);
        foreach (var fee in existing)
        {
            fee.Deactivate();
            examinationFeeRepository.Update(fee);
        }

        var examinationFee = ExaminationFee.Create(command.ExaminationTypeId, role, command.Amount, command.IsPercentage);

        await examinationFeeRepository.AddAsync(examinationFee, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(examinationFee.Adapt<ExaminationFeeDto>());
    }

    private static async Task<IReadOnlyList<ExaminationFee>> FindActiveAsync(
        IExaminationFeeRepository repository,
        Guid examinationTypeId,
        ExamFeeRole role,
        CancellationToken ct)
    {
        var spec = new DynamicSpecification<ExaminationFee>();
        spec.AddCriteria(f => f.ExaminationTypeId == examinationTypeId);
        spec.AddCriteria(f => f.Role == role);
        spec.AddCriteria(f => f.IsActive);
        return await repository.FindAsync(spec, ct);
    }
}