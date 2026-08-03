using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Entities;
using RadiologyCenter.ResourceManagement.Infrastructure.Persistence;

namespace RadiologyCenter.ResourceManagement.Infrastructure.Repositories;

public class ReferralDoctorRepository : BaseRepository<ReferralDoctor, Guid>, IReferralDoctorRepository
{
    public ReferralDoctorRepository(ResourceManagementDbContext context) : base(context) { }
}
