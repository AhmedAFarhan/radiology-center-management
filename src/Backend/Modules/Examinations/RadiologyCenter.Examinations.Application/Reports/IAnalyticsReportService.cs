using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Reports;

public interface IAnalyticsReportService
{
    ReportContentDto ExportFinancial(FinancialAnalyticsDto data, DateTime from, DateTime to);
    ReportContentDto ExportOperational(OperationalAnalyticsDto data, DateTime from, DateTime to);
    ReportContentDto ExportStaffMachine(StaffMachineAnalyticsDto data, DateTime from, DateTime to);
    ReportContentDto ExportProfit(ProfitAnalyticsDto data, DateTime from, DateTime to);
    ReportContentDto ExportInsurance(InsuranceAnalyticsDto data, DateTime from, DateTime to);
    ReportContentDto ExportCashFlow(CashFlowReportDto data, DateTime from, DateTime to);
}
