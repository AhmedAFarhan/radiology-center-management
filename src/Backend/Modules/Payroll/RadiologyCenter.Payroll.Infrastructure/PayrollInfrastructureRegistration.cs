using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence.Interceptors;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Infrastructure.Adapters;
using RadiologyCenter.Payroll.Infrastructure.Persistence;
using RadiologyCenter.Payroll.Infrastructure.Repositories;
using RadiologyCenter.Payroll.Infrastructure.Services;

namespace RadiologyCenter.Payroll.Infrastructure;

public static class PayrollInfrastructureRegistration
{
    public static IServiceCollection AddPayrollInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<PayrollDbContext>((sp, options) =>
            options.UseSqlServer(connectionString)
                   .AddInterceptors(sp.GetRequiredService<AuditSoftDeleteInterceptor>())
                   .AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>())
);

        services.AddScoped<ISalaryComponentRepository, SalaryComponentRepository>();
        services.AddScoped<ISalaryRepository, SalaryRepository>();
        services.AddScoped<IAllowanceAssignmentRepository, AllowanceAssignmentRepository>();
        services.AddScoped<IExaminationFeeRepository, ExaminationFeeRepository>();
        services.AddScoped<IReferralFeeRepository, ReferralFeeRepository>();
        services.AddScoped<IPayRunRepository, PayRunRepository>();
        services.AddScoped<IPayrollUnitOfWork, PayrollUnitOfWork>();
        services.AddScoped<IPayslipPdfService, PayslipPdfService>();
        services.AddScoped<IReferralFeeStatementPdfService, ReferralFeeStatementPdfService>();

        services.AddScoped<IPayrollStaffDirectory, PayrollStaffDirectory>();
        services.AddScoped<IReferralDoctorDirectory, ReferralDoctorDirectory>();
        services.AddScoped<IStaffLeaveResolver, StaffLeaveDaysResolver>();
        services.AddScoped<IStaffWorkHoursResolver, StaffWorkHoursResolver>();
        services.AddScoped<IExaminationTypeDirectory, ExaminationTypeDirectory>();

        return services;
    }
}
