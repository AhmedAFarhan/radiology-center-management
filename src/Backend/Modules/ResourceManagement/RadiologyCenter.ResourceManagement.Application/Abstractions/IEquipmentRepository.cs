using RadiologyCenter.ResourceManagement.Domain.Entities;

namespace RadiologyCenter.ResourceManagement.Application.Abstractions;

public interface IEquipmentRepository : IBaseRepository<Equipment, Guid>;
