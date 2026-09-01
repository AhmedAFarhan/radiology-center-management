using RadiologyCenter.BuildingBlocks.Application.Excel;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Reports;

public sealed class AnalyticsReportService : IAnalyticsReportService
{
    private readonly IExcelService _excel;

    public AnalyticsReportService(IExcelService excel) => _excel = excel;

    public ReportContentDto ExportFinancial(FinancialAnalyticsDto data, DateTime from, DateTime to)
    {
        var rows = new List<FinancialReportRow>();
        foreach (var point in data.RevenueByMonth)
            rows.Add(new(point.Month, "Monthly", point.Collected, point.Billed, 0, 0, 0));
        foreach (var modality in data.RevenueByModality)
            rows.Add(new(modality.Modality, "Modality", modality.Collected, 0, 0, 0, modality.ExamCount));

        var columns = new List<ExcelColumn<FinancialReportRow>>
        {
            new("Analytics.Label", "Label", r => r.Label, ExcelColumnType.Text, 25),
            new("Analytics.Category", "Category", r => r.Category, ExcelColumnType.Text, 15),
            new("Analytics.Collected", "Collected", r => r.Collected, ExcelColumnType.Currency, 18),
            new("Analytics.Billed", "Billed", r => r.Billed, ExcelColumnType.Currency, 18),
            new("Analytics.Discounts", "Discounts", r => r.Discounts, ExcelColumnType.Currency, 18),
            new("Analytics.Receivables", "Receivables", r => r.Receivables, ExcelColumnType.Currency, 18),
            new("Analytics.ExamCount", "Exam Count", r => r.ExamCount, ExcelColumnType.Number, 14),
        };

        var content = _excel.Export("Financial Report", $"FinancialReport_{from:yyyyMMdd}-{to:yyyyMMdd}", columns, rows);
        return new ReportContentDto(content, $"FinancialReport_{from:yyyyMMdd}-{to:yyyyMMdd}.xlsx", ExcelContentTypes.Xlsx);
    }

    public ReportContentDto ExportOperational(OperationalAnalyticsDto data, DateTime from, DateTime to)
    {
        var funnelRows = data.Funnel.Select(f => new OperationalReportRow(f.Status, f.Count)).ToList();
        var monthRows = data.VolumeByMonth.Select(m => new OperationalReportRow(m.Month, m.Total)).ToList();
        var modalityRows = data.VolumeByModality.Select(v => new OperationalReportRow(v.Modality, v.Total)).ToList();
        var allRows = funnelRows.Concat(monthRows).Concat(modalityRows);

        var columns = new List<ExcelColumn<OperationalReportRow>>
        {
            new("Analytics.Label", "Label", r => r.Label, ExcelColumnType.Text, 25),
            new("Analytics.Count", "Count", r => r.Count, ExcelColumnType.Number, 14),
        };

        var content = _excel.Export("Operational Report", $"OperationalReport_{from:yyyyMMdd}-{to:yyyyMMdd}", columns, allRows);
        return new ReportContentDto(content, $"OperationalReport_{from:yyyyMMdd}-{to:yyyyMMdd}.xlsx", ExcelContentTypes.Xlsx);
    }

    public ReportContentDto ExportStaffMachine(StaffMachineAnalyticsDto data, DateTime from, DateTime to)
    {
        var rows = data.Radiologists.Select(s => new StaffReportRow(s.Name, "Radiologist", s.CompletedExams, s.FeeIncome))
            .Concat(data.Technicians.Select(s => new StaffReportRow(s.Name, "Technician", s.CompletedExams, s.FeeIncome)))
            .Concat(data.ReferralDoctors.Select(r => new StaffReportRow(r.Name, "Referral Doctor", r.ReferredExams, r.ReferralFeeIncome)));

        var columns = new List<ExcelColumn<StaffReportRow>>
        {
            new("Analytics.Name", "Name", r => r.Name, ExcelColumnType.Text, 25),
            new("Analytics.Role", "Role", r => r.Role, ExcelColumnType.Text, 18),
            new("Analytics.CompletedExams", "Completed Exams", r => r.CompletedExams, ExcelColumnType.Number, 16),
            new("Analytics.FeeIncome", "Fee Income", r => r.FeeIncome, ExcelColumnType.Currency, 18),
        };

        var content = _excel.Export("Staff Report", $"StaffReport_{from:yyyyMMdd}-{to:yyyyMMdd}", columns, rows);
        return new ReportContentDto(content, $"StaffReport_{from:yyyyMMdd}-{to:yyyyMMdd}.xlsx", ExcelContentTypes.Xlsx);
    }

    public ReportContentDto ExportProfit(ProfitAnalyticsDto data, DateTime from, DateTime to)
    {
        var rows = new[]
        {
            new ProfitReportRow("Revenue Collected", data.RevenueCollected),
            new ProfitReportRow("Total Billed", data.TotalBilled),
            new ProfitReportRow("Discounts", data.Discounts),
            new ProfitReportRow("Staff Case Fees", data.StaffCaseFees),
            new ProfitReportRow("Referral Fees", data.ReferralFees),
            new ProfitReportRow("Labor Costs", data.LaborCosts),
            new ProfitReportRow("Material Costs", data.MaterialCosts),
            new ProfitReportRow("Total Costs", data.TotalCosts),
            new ProfitReportRow("Net Profit", data.NetProfit),
        };

        var columns = new List<ExcelColumn<ProfitReportRow>>
        {
            new("Analytics.Item", "Item", r => r.Item, ExcelColumnType.Text, 30),
            new("Analytics.Amount", "Amount", r => r.Amount, ExcelColumnType.Currency, 20),
        };

        var content = _excel.Export("Profit Report", $"ProfitReport_{from:yyyyMMdd}-{to:yyyyMMdd}", columns, rows);
        return new ReportContentDto(content, $"ProfitReport_{from:yyyyMMdd}-{to:yyyyMMdd}.xlsx", ExcelContentTypes.Xlsx);
    }

    public ReportContentDto ExportInsurance(InsuranceAnalyticsDto data, DateTime from, DateTime to)
    {
        var rows = data.ClaimRows.Select(c => new InsuranceClaimExcelRow(
            c.PatientName, c.InsuranceCompany, c.PolicyNumber,
            c.BilledAmount, c.PayerShare, c.PatientShare,
            c.Status, c.SubmittedAt, c.ApprovedAt,
            c.SettledAmount, c.RemainingOwed));

        var columns = new List<ExcelColumn<InsuranceClaimExcelRow>>
        {
            new("Analytics.PatientName", "Patient", r => r.PatientName, ExcelColumnType.Text, 22),
            new("Analytics.Company", "Company", r => r.Company, ExcelColumnType.Text, 20),
            new("Analytics.PolicyNumber", "Policy #", r => r.PolicyNumber, ExcelColumnType.Text, 18),
            new("Analytics.BilledAmount", "Billed", r => r.BilledAmount, ExcelColumnType.Currency, 16),
            new("Analytics.PayerShare", "Payer Share", r => r.PayerShare, ExcelColumnType.Currency, 16),
            new("Analytics.PatientShare", "Patient Share", r => r.PatientShare, ExcelColumnType.Currency, 16),
            new("Analytics.Status", "Status", r => r.Status, ExcelColumnType.Text, 14),
            new("Analytics.Settled", "Settled", r => r.SettledAmount, ExcelColumnType.Currency, 16),
            new("Analytics.Remaining", "Remaining", r => r.RemainingOwed, ExcelColumnType.Currency, 16),
        };

        var content = _excel.Export("Insurance Claims", $"InsuranceReport_{from:yyyyMMdd}-{to:yyyyMMdd}", columns, rows);
        return new ReportContentDto(content, $"InsuranceReport_{from:yyyyMMdd}-{to:yyyyMMdd}.xlsx", ExcelContentTypes.Xlsx);
    }

    public ReportContentDto ExportCashFlow(CashFlowReportDto data, DateTime from, DateTime to)
    {
        var rows = data.ByMonth.Select(m => new CashFlowExcelRow(m.Month, "Monthly", m.Inflows, m.Outflows, m.Net))
            .Concat(data.ByReason.Select(r => new CashFlowExcelRow(r.Reason, "By Reason", r.InflowAmount, r.OutflowAmount, r.InflowAmount - r.OutflowAmount)));

        var columns = new List<ExcelColumn<CashFlowExcelRow>>
        {
            new("Analytics.Label", "Label", r => r.Label, ExcelColumnType.Text, 25),
            new("Analytics.Category", "Category", r => r.Category, ExcelColumnType.Text, 15),
            new("Analytics.Inflows", "Inflows", r => r.Inflows, ExcelColumnType.Currency, 18),
            new("Analytics.Outflows", "Outflows", r => r.Outflows, ExcelColumnType.Currency, 18),
            new("Analytics.Net", "Net", r => r.Net, ExcelColumnType.Currency, 18),
        };

        var content = _excel.Export("Cash Flow", $"CashFlowReport_{from:yyyyMMdd}-{to:yyyyMMdd}", columns, rows);
        return new ReportContentDto(content, $"CashFlowReport_{from:yyyyMMdd}-{to:yyyyMMdd}.xlsx", ExcelContentTypes.Xlsx);
    }

    private sealed record FinancialReportRow(string Label, string Category, decimal Collected, decimal Billed, decimal Discounts, decimal Receivables, int ExamCount);
    private sealed record OperationalReportRow(string Label, int Count);
    private sealed record StaffReportRow(string Name, string Role, int CompletedExams, decimal FeeIncome);
    private sealed record ProfitReportRow(string Item, decimal Amount);
    private sealed record InsuranceClaimExcelRow(string PatientName, string Company, string PolicyNumber, decimal BilledAmount, decimal PayerShare, decimal PatientShare, string Status, DateTime? SubmittedAt, DateTime? ApprovedAt, decimal SettledAmount, decimal RemainingOwed);
    private sealed record CashFlowExcelRow(string Label, string Category, decimal Inflows, decimal Outflows, decimal Net);
}
