using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Infrastructure.Persistence;

namespace RadiologyCenter.Examinations.Infrastructure.Repositories;

public class ExaminationTypeItemRepository : IExaminationTypeItemRepository
{
    private readonly ExaminationsDbContext _context;

    public ExaminationTypeItemRepository(ExaminationsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ExaminationTypeItem>> GetByTypeIdAsync(Guid examinationTypeId, CancellationToken ct = default) =>
        await _context.ExaminationTypeItems
            .Where(i => i.ExaminationTypeId == examinationTypeId)
            .ToListAsync(ct);

    public async Task<ExaminationTypeItem?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.ExaminationTypeItems.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<bool> ExistsByItemAsync(Guid examinationTypeId, Guid itemId, CancellationToken ct = default) =>
        await _context.ExaminationTypeItems.AnyAsync(i => i.ExaminationTypeId == examinationTypeId && i.ItemId == itemId, ct);

    public async Task AddAsync(ExaminationTypeItem item, CancellationToken ct = default)
    {
        await _context.ExaminationTypeItems.AddAsync(item, ct);
    }

    public void Remove(ExaminationTypeItem item) => _context.ExaminationTypeItems.Remove(item);
}