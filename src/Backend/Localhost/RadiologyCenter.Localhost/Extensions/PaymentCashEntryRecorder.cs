using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Cash.Application.Commands.Sessions.AddCashEntry;
using RadiologyCenter.Cash.Application.Commands.Sessions.Common;
using RadiologyCenter.Cash.Application.Commands.Sessions.OpenCashSession;
using RadiologyCenter.Cash.Application.DTOs;
using RadiologyCenter.Cash.Application.Queries.Sessions.GetMyOpenCashSession;
using RadiologyCenter.Examinations.Application.Abstractions;
using Wolverine;

namespace RadiologyCenter.Localhost.Extensions;

public class PaymentCashEntryRecorder : IPaymentCashEntryRecorder
{
    private readonly IMessageBus _bus;

    public PaymentCashEntryRecorder(IMessageBus bus)
    {
        _bus = bus;
    }

    public async Task<Result> RecordAsync(Guid examinationId, decimal amount, string? description, CancellationToken ct)
    {
        var sessionResult = await _bus.InvokeAsync<Result<CashSessionDto?>>(new GetMyOpenCashSessionQuery(), ct);
        if (sessionResult.IsFailure)
            return Result.Failure(sessionResult.Error!);

        Guid sessionId;
        if (sessionResult.Value is null)
        {
            var openResult = await _bus.InvokeAsync<Result<CashSessionDto>>(new OpenCashSessionCommand(0), ct);
            if (openResult.IsFailure)
                return Result.Failure(openResult.Error!);

            sessionId = openResult.Value.Id;
        }
        else
        {
            sessionId = sessionResult.Value.Id;
        }

        var entryCommand = new AddCashEntryCommand(
            sessionId,
            CashDirectionInput.In,
            CashReasonInput.Payment,
            amount,
            description,
            examinationId.ToString());

        var entryResult = await _bus.InvokeAsync<Result<CashEntryDto>>(entryCommand, ct);
        if (entryResult.IsFailure)
            return Result.Failure(entryResult.Error!);

        return Result.Success();
    }
}