using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Commands.CreateVisit;

public static class CreateVisitCommandHandler
{
    public static async Task<Result<VisitDto>> HandleAsync(
        CreateVisitCommand command,
        IVisitRepository visitRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var visit = Visit.Create(
            command.PatientId,
            command.VisitedAt,
            command.AppointmentId,
            command.Notes);

        await visitRepository.AddAsync(visit, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(VisitMapper.Map(visit, new Dictionary<Guid, string>()));
    }
}
