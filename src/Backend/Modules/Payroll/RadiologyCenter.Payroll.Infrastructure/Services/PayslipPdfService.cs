using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Application.DTOs;
using RadiologyCenter.Payroll.Domain.Entities;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.ResourceManagement.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Infrastructure.Services;

public class PayslipPdfService : IPayslipPdfService
{
    private readonly IPayRunRepository _payRunRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IExamFeeIncomeResolver _examFeeIncomeResolver;

    public PayslipPdfService(
        IPayRunRepository payRunRepository,
        IStaffRepository staffRepository,
        IExamFeeIncomeResolver examFeeIncomeResolver)
    {
        _payRunRepository = payRunRepository;
        _staffRepository = staffRepository;
        _examFeeIncomeResolver = examFeeIncomeResolver;
    }

    public async Task<byte[]> GeneratePayslipPdfAsync(Guid payRunId, Guid staffId, CancellationToken ct = default)
    {
        var payRun = await _payRunRepository.GetWithPayslipsAsync(payRunId, ct)
            ?? throw new InvalidOperationException("Pay run not found.");

        var payslip = payRun.Payslips.FirstOrDefault(p => p.StaffId == staffId)
            ?? throw new InvalidOperationException("Payslip not found for this staff member.");

        var staff = await _staffRepository.GetByIdAsync(staffId, ct)
            ?? throw new InvalidOperationException("Staff member not found.");

        var breakdown = await _examFeeIncomeResolver.GetFeeIncomeBreakdownAsync(staffId, payRun.RunFrom, payRun.RunTo, ct);

        var dto = new PayslipPdfDto
        {
            StaffFullName = staff.FullName,
            StaffPosition = staff.Position.Name,
            StaffDepartment = staff.Department,
            StaffSpecialization = staff.Specialization,
            StaffPhoneNumber = staff.PhoneNumber,
            StaffHireDate = staff.HireDate,
            SalaryCalculationRule = staff.SalaryCalculationRule?.Name ?? ResourceManagement.Domain.Enumerations.SalaryCalculationRule.FixedPlusFees.Name,
            RunFrom = payRun.RunFrom,
            RunTo = payRun.RunTo,
            PayRunStatus = payRun.Status.Name,
            GrossSalary = payslip.GrossSalary,
            UnpaidLeaveDays = payslip.UnpaidLeaveDays,
            UnpaidLeaveDeduction = payslip.UnpaidLeaveDeduction,
            Components = payslip.Components.Select(c => new PayslipComponentDto(c.Id, c.Name, c.Amount, c.IsDeduction)).ToList(),
            TotalEarnings = payslip.TotalEarnings,
            TotalDeductions = payslip.TotalDeductions,
            NetSalary = payslip.NetSalary,
            ExaminationFeeBreakdown = breakdown.Items.Select(i => new ExamFeeBreakdownItemDto(i.ExaminationTypeName, i.Count, i.FeeRate, i.Total)).ToList(),
            ExaminationFeeTotal = breakdown.TotalIncome
        };

        return BuildPdf(dto);
    }

    private static byte[] BuildPdf(PayslipPdfDto dto)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(30);
                page.MarginVertical(20);

                page.Header().Element(h => BuildHeader(h, dto));
                page.Content().Element(c => BuildContent(c, dto));
                page.Footer().Element(BuildFooter);
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    private static void BuildHeader(IContainer container, PayslipPdfDto dto)
    {
        container.Row(row =>
        {
            row.RelativeItem().Width(50).Column(col =>
            {
                col.Item().Image(GetLogoBytes());
            });

            row.RelativeItem(3).Column(col =>
            {
                col.Item().Text(BrandConstants.CompanyName).FontSize(18).Bold().FontColor(BrandConstants.PrimaryColor);
                col.Item().Text(PdfLabels.Payslip).FontSize(14).Bold().FontColor(Colors.Grey.Darken1);
                col.Item().PaddingTop(5).Text($"Period: {dto.RunFrom:MMM dd, yyyy} - {dto.RunTo:MMM dd, yyyy}").FontSize(9).FontColor(Colors.Grey.Medium);
            });

            row.RelativeItem(1).Column(col =>
            {
                col.Item().AlignRight().Text("Status").FontSize(8).FontColor(Colors.Grey.Medium);
                col.Item().AlignRight().Text(dto.PayRunStatus).FontSize(10).Bold().FontColor(GetStatusColor(dto.PayRunStatus));
            });
        });
    }

    private static byte[] GetLogoBytes()
    {
        var assembly = typeof(PayslipPdfService).Assembly;
        using var stream = assembly.GetManifestResourceStream(BrandConstants.LogoResourceName);
        if (stream is null)
            return [];

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    private static void BuildContent(IContainer container, PayslipPdfDto dto)
    {
        container.PaddingTop(10).Column(col =>
        {
            col.Item().Background(Colors.Grey.Lighten5).Padding(10).Row(row =>
            {
                row.RelativeItem(2).Column(c => c.Item().Text(PdfLabels.EmployeeInformation).FontSize(11).Bold().FontColor(BrandConstants.PrimaryColor));
                row.RelativeItem(1).Column(c => c.Item().Text(PdfLabels.PayPeriod).FontSize(11).Bold().FontColor(BrandConstants.PrimaryColor));
            });

            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

            col.Item().Padding(10).Row(row =>
            {
                row.RelativeItem(2).Column(c =>
                {
                    c.Item().Text($"Name: {dto.StaffFullName}").FontSize(9);
                    c.Item().Text($"Position: {dto.StaffPosition}").FontSize(9);
                    c.Item().Text($"Phone: {dto.StaffPhoneNumber}").FontSize(9);
                });
                row.RelativeItem(1).Column(c =>
                {
                    c.Item().Text($"Period: {dto.RunFrom:yyyy-MM-dd} to {dto.RunTo:yyyy-MM-dd}").FontSize(9);
                    c.Item().Text($"Status: {dto.PayRunStatus}").FontSize(9);
                });
            });

            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

            BuildEarningsSection(col.Item(), dto);

            if (dto.ExaminationFeeBreakdown.Count > 0)
                col.Item().PaddingTop(10).Element(c => BuildExamFeeBreakdown(c, dto));

            col.Item().PaddingTop(10).Element(c => BuildDeductions(c, dto));
            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            BuildNetSalary(col.Item(), dto);

            if (!string.IsNullOrWhiteSpace(dto.StaffDepartment) || !string.IsNullOrWhiteSpace(dto.StaffSpecialization) || !string.IsNullOrWhiteSpace(dto.SalaryCalculationRule))
            {
                col.Item().PaddingTop(20).Padding(10).Column(c =>
                {
                    c.Item().Text(PdfLabels.AdditionalInformation).FontSize(10).Bold().FontColor(Colors.Grey.Darken1);
                    if (!string.IsNullOrWhiteSpace(dto.StaffDepartment))
                        c.Item().PaddingTop(3).Text($"{PdfLabels.Department} {dto.StaffDepartment}").FontSize(9);
                    if (!string.IsNullOrWhiteSpace(dto.StaffSpecialization))
                        c.Item().Text($"{PdfLabels.Specialization} {dto.StaffSpecialization}").FontSize(9);
                    if (!string.IsNullOrWhiteSpace(dto.SalaryCalculationRule))
                        c.Item().Text($"{PdfLabels.SalaryRule} {dto.SalaryCalculationRule}").FontSize(9);
                });
            }
        });
    }

    private static void BuildEarningsSection(IContainer container, PayslipPdfDto dto)
    {
        container.Padding(10).Column(col =>
        {
            col.Item().Text(PdfLabels.Earnings).FontSize(11).Bold().FontColor(Colors.Green.Darken1);
            col.Item().PaddingTop(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                });
                table.Header(h =>
                {
                    h.Cell().Text(PdfLabels.Description).FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                    h.Cell().AlignRight().Text(PdfLabels.Amount).FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                });
                table.Cell().Text(PdfLabels.BaseSalary).FontSize(9);
                table.Cell().AlignRight().Text(dto.GrossSalary.ToString("N2")).FontSize(9);
                foreach (var c in dto.Components.Where(c => !c.IsDeduction && c.Name != "Examination Fees"))
                {
                    table.Cell().Text(c.Name).FontSize(9);
                    table.Cell().AlignRight().Text(c.Amount.ToString("N2")).FontSize(9);
                }
                if (dto.ExaminationFeeTotal > 0)
                {
                    table.Cell().Text("Examination Fees").FontSize(9).Bold();
                    table.Cell().AlignRight().Text(dto.ExaminationFeeTotal.ToString("N2")).FontSize(9).Bold();
                }
                table.Cell().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                table.Cell().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                table.Cell().Text(PdfLabels.TotalEarnings).FontSize(9).Bold();
                table.Cell().AlignRight().Text(dto.TotalEarnings.ToString("N2")).FontSize(9).Bold().FontColor(Colors.Green.Darken1);
            });
        });
    }

    private static void BuildExamFeeBreakdown(IContainer container, PayslipPdfDto dto)
    {
        container.Padding(10).BorderLeft(2).BorderColor(BrandConstants.PrimaryColor).Column(col =>
        {
            col.Item().Text(PdfLabels.ExaminationFeeBreakdown).FontSize(10).Bold().FontColor(BrandConstants.PrimaryColor);
            col.Item().PaddingTop(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });
                table.Header(h =>
                {
                    h.Cell().Text(PdfLabels.ExamType).FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                    h.Cell().AlignRight().Text(PdfLabels.Count).FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                    h.Cell().AlignRight().Text(PdfLabels.Rate).FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                    h.Cell().AlignRight().Text(PdfLabels.Total).FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                });
                foreach (var item in dto.ExaminationFeeBreakdown)
                {
                    table.Cell().Text(item.ExaminationTypeName).FontSize(9);
                    table.Cell().AlignRight().Text(item.Count.ToString()).FontSize(9);
                    table.Cell().AlignRight().Text(item.FeeRate.ToString("N2")).FontSize(9);
                    table.Cell().AlignRight().Text(item.Total.ToString("N2")).FontSize(9);
                }
                table.Cell().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                table.Cell().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                table.Cell().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                table.Cell().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                table.Cell().Text(PdfLabels.TotalExamFees).FontSize(9).Bold();
                table.Cell().AlignRight().Text(dto.ExaminationFeeBreakdown.Sum(i => i.Count).ToString()).FontSize(9).Bold();
                table.Cell().AlignRight();
                table.Cell().AlignRight().Text(dto.ExaminationFeeTotal.ToString("N2")).FontSize(9).Bold().FontColor(BrandConstants.PrimaryColor);
            });
        });
    }

    private static void BuildDeductions(IContainer container, PayslipPdfDto dto)
    {
        container.Padding(10).Column(col =>
        {
            col.Item().Text(PdfLabels.Deductions).FontSize(11).Bold().FontColor(Colors.Red.Darken1);
            col.Item().PaddingTop(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                });
                table.Header(h =>
                {
                    h.Cell().Text(PdfLabels.Description).FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                    h.Cell().AlignRight().Text(PdfLabels.Amount).FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                });
                if (dto.UnpaidLeaveDays > 0)
                {
                    table.Cell().Text($"Unpaid Leave ({dto.UnpaidLeaveDays} days)").FontSize(9);
                    table.Cell().AlignRight().Text(dto.UnpaidLeaveDeduction.ToString("N2")).FontSize(9);
                }
                foreach (var c in dto.Components.Where(c => c.IsDeduction))
                {
                    table.Cell().Text(c.Name).FontSize(9);
                    table.Cell().AlignRight().Text(c.Amount.ToString("N2")).FontSize(9);
                }
                table.Cell().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                table.Cell().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                table.Cell().Text(PdfLabels.TotalDeductions).FontSize(9).Bold();
                table.Cell().AlignRight().Text(dto.TotalDeductions.ToString("N2")).FontSize(9).Bold().FontColor(Colors.Red.Darken1);
            });
        });
    }

    private static void BuildNetSalary(IContainer container, PayslipPdfDto dto)
    {
        container.Background(Colors.Grey.Lighten5).Padding(15).Row(row =>
        {
            row.RelativeItem(2).Column(c => c.Item().Text(PdfLabels.NetSalary).FontSize(14).Bold().FontColor(BrandConstants.PrimaryColor));
            row.RelativeItem(1).Column(c => c.Item().AlignRight().Text(dto.NetSalary.ToString("N2")).FontSize(16).Bold().FontColor(BrandConstants.PrimaryColor));
        });
    }

    private static void BuildFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span(PdfLabels.GeneratedOn).FontSize(8).FontColor(Colors.Grey.Medium);
            text.Span(DateTime.Now.ToString("MMM dd, yyyy HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
            text.Span($" | {PdfLabels.SystemGeneratedDocument}").FontSize(8).FontColor(Colors.Grey.Medium);
        });
    }

    private static string GetStatusColor(string status) =>
        status == PayRunStatus.Draft.Name ? Colors.Grey.Medium :
        status == PayRunStatus.Computed.Name ? BrandConstants.PrimaryColor :
        status == PayRunStatus.Approved.Name ? Colors.Green.Medium :
        status == PayRunStatus.Paid.Name ? Colors.Green.Darken2 :
        status == PayRunStatus.Rejected.Name ? Colors.Red.Medium :
        Colors.Grey.Medium;
}
