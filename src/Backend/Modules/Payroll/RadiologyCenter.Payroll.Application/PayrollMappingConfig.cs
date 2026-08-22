using Mapster;
using RadiologyCenter.BuildingBlocks.Application.Localization;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Domain.Entities;

namespace RadiologyCenter.Payroll.Application;

public static class PayrollMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<SalaryComponent, SalaryComponentDto>.NewConfig()
            .Map(d => d.Kind, s => s.Kind.LocalizedName())
            .Map(d => d.KindKey, s => s.Kind.Name)
            .Map(d => d.Frequency, s => s.Frequency == null ? null : s.Frequency.LocalizedName())
            .Map(d => d.FrequencyKey, s => s.Frequency == null ? null : s.Frequency.Name);

        TypeAdapterConfig<Salary, SalaryDto>.NewConfig()
            .Map(d => d.SalaryType, s => s.SalaryType.LocalizedName())
            .Map(d => d.SalaryTypeKey, s => s.SalaryType.Name);

        TypeAdapterConfig<AllowanceAssignment, AllowanceAssignmentDto>.NewConfig()
            .Map(d => d.Frequency, s => s.Frequency == null ? null : s.Frequency.LocalizedName())
            .Map(d => d.FrequencyKey, s => s.Frequency == null ? null : s.Frequency.Name);

        TypeAdapterConfig<ExaminationFee, ExaminationFeeDto>.NewConfig()
            .Map(d => d.Role, s => s.Role.LocalizedName())
            .Map(d => d.RoleKey, s => s.Role.Name);

        TypeAdapterConfig<ReferralFee, ReferralFeeDto>.NewConfig();

        TypeAdapterConfig<PayRun, PayRunDto>.NewConfig()
            .Map(d => d.Status, s => s.Status.LocalizedName())
            .Map(d => d.StatusKey, s => s.Status.Name)
            .Map(d => d.Payslips, s => s.Payslips);

        TypeAdapterConfig<Payslip, PayslipDto>.NewConfig()
            .Map(d => d.TotalEarnings, s => s.TotalEarnings)
            .Map(d => d.TotalDeductions, s => s.TotalDeductions)
            .Map(d => d.NetSalary, s => s.NetSalary)
            .Map(d => d.Components, s => s.Components);

        TypeAdapterConfig<PayslipComponent, PayslipComponentDto>.NewConfig();
    }
}