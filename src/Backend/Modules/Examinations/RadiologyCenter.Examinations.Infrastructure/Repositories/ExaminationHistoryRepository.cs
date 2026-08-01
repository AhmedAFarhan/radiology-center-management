using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Infrastructure.Persistence;

namespace RadiologyCenter.Examinations.Infrastructure.Repositories;

public class ExaminationHistoryRepository : BaseRepository<ExaminationHistory, Guid>, IExaminationHistoryRepository
{
    public ExaminationHistoryRepository(ExaminationsDbContext context) : base(context) { }
}
