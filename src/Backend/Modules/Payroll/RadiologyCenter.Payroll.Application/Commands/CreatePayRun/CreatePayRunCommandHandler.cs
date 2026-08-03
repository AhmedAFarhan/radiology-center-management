using Mapster;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;

namespace RadiologyCenter.Payroll.Application.Commands.CreatePayRun;

public static class CreatePayRunCommandHandler
{
    public static async Task<Result<PayRunDto>> HandleAsync(
        CreatePayRunCommand command,
        IPayRunRepository payRunRepository,
        IPayrollUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (await payRunRepository.ExistsOverlappingAsync(command.RunFrom, command.RunTo, ct))
            return Result.Failure<PayRunDto>(Error.Conflict($"A pay run overlapping '{command.RunFrom:yyyy-MM-dd}' to '{command.RunTo:yyyy-MM-dd}' already exists."));

        var payRun = PayRun.Create(command.RunFrom, command.RunTo, command.Notes);

        await payRunRepository.AddAsync(payRun, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(payRun.Adapt<PayRunDto>());
    }
}