using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Enumerations;
using RadiologyCenter.ResourceManagement.Application.Abstractions;

namespace RadiologyCenter.Payroll.Infrastructure.Services;

public class ReferralFeeStatementPdfService : IReferralFeeStatementPdfService
{
    private readonly IPayRunRepository _payRunRepository;
    private readonly IReferralDoctorRepository _referralDoctorRepository;
    private readonly IReferralFeeStatementResolver _resolver;

    public ReferralFeeStatementPdfService(
        IPayRunRepository payRunRepository,
        IReferralDoctorRepository referralDoctorRepository,
        IReferralFeeStatementResolver resolver)
    {
        _payRunRepository = payRunRepository;
        _referralDoctorRepository = referralDoctorRepository;
        _resolver = resolver;
    }

    public async Task<byte[]> GenerateStatementPdfAsync(
        Guid payRunId,
        Guid referralDoctorId,
        CancellationToken ct = default)
    {
        var payRun = await _payRunRepository.GetWithPayslipsAndReferralStatementsAsync(payRunId, ct)
            ?? throw new InvalidOperationException("Pay run not found.");

        var statement = payRun.ReferralFeeStatements.FirstOrDefault(s => s.ReferralDoctorId == referralDoctorId)
            ?? throw new InvalidOperationException("Referral fee statement not found for this doctor.");

        var doctor = await _referralDoctorRepository.GetByIdAsync(referralDoctorId, ct)
            ?? throw new InvalidOperationException("Referral doctor not found.");

        var breakdown = await _resolver.GetReferralFeeBreakdownAsync(referralDoctorId, payRun.RunFrom, payRun.RunTo, ct);

        return BuildPdf(doctor.FullName, doctor.Phone, doctor.Email, doctor.Specialization, payRun, statement, breakdown);
    }

    private static byte[] BuildPdf(
        string doctorName,
        string phone,
        string? email,
        string? specialization,
        Domain.Entities.PayRun payRun,
        Domain.Entities.ReferralFeeStatement statement,
        ReferralFeeExamBreakdown? breakdown)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(30);
                page.MarginVertical(20);

                page.Header().Element(h => BuildHeader(h, doctorName, payRun));
                page.Content().Element(c => BuildContent(c, doctorName, phone, email, specialization, payRun, statement, breakdown));
                page.Footer().Element(BuildFooter);
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    private static void BuildHeader(IContainer container, string doctorName, Domain.Entities.PayRun payRun)
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
                col.Item().Text(PdfLabels.ReferralFeeStatement).FontSize(14).Bold().FontColor(Colors.Grey.Darken1);
                col.Item().PaddingTop(5).Text($"Period: {payRun.RunFrom:MMM dd, yyyy} - {payRun.RunTo:MMM dd, yyyy}").FontSize(9).FontColor(Colors.Grey.Medium);
            });

            row.RelativeItem(1).Column(col =>
            {
                col.Item().AlignRight().Text("Status").FontSize(8).FontColor(Colors.Grey.Medium);
                col.Item().AlignRight().Text(payRun.Status.Name).FontSize(10).Bold().FontColor(GetStatusColor(payRun.Status.Name));
            });
        });
    }

    private static byte[] GetLogoBytes()
    {
        var assembly = typeof(ReferralFeeStatementPdfService).Assembly;
        using var stream = assembly.GetManifestResourceStream(BrandConstants.LogoResourceName);
        if (stream is null)
            return [];

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    private static void BuildContent(
        IContainer container,
        string doctorName,
        string phone,
        string? email,
        string? specialization,
        Domain.Entities.PayRun payRun,
        Domain.Entities.ReferralFeeStatement statement,
        ReferralFeeExamBreakdown? breakdown)
    {
        container.PaddingTop(10).Column(col =>
        {
            col.Item().Background(Colors.Grey.Lighten5).Padding(10).Row(row =>
            {
                row.RelativeItem(2).Column(c => c.Item().Text(PdfLabels.ReferralDoctorInformation).FontSize(11).Bold().FontColor(BrandConstants.PrimaryColor));
                row.RelativeItem(1).Column(c => c.Item().Text(PdfLabels.StatementPeriod).FontSize(11).Bold().FontColor(BrandConstants.PrimaryColor));
            });

            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

            col.Item().Padding(10).Row(row =>
            {
                row.RelativeItem(2).Column(c =>
                {
                    c.Item().Text($"Doctor: {doctorName}").FontSize(9);
                    c.Item().Text($"Phone: {phone}").FontSize(9);
                    if (!string.IsNullOrWhiteSpace(email))
                        c.Item().Text($"Email: {email}").FontSize(9);
                    if (!string.IsNullOrWhiteSpace(specialization))
                        c.Item().Text($"Specialization: {specialization}").FontSize(9);
                });
                row.RelativeItem(1).Column(c =>
                {
                    c.Item().Text($"Period: {payRun.RunFrom:yyyy-MM-dd} to {payRun.RunTo:yyyy-MM-dd}").FontSize(9);
                    c.Item().Text($"Status: {payRun.Status.Name}").FontSize(9);
                });
            });

            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

            if (breakdown is not null && breakdown.Items.Count > 0)
            {
                col.Item().Padding(10).Column(c =>
                {
                    c.Item().Text(PdfLabels.ExaminationFeeBreakdown).FontSize(11).Bold().FontColor(Colors.Grey.Darken1);
                    c.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });
                        table.Header(h =>
                        {
                            h.Cell().Text(PdfLabels.ExamType).FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                            h.Cell().AlignRight().Text(PdfLabels.Count).FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                            h.Cell().AlignRight().Text("Total Fee").FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                        });
                        foreach (var item in breakdown.Items)
                        {
                            table.Cell().Text(item.ExaminationTypeName).FontSize(9);
                            table.Cell().AlignRight().Text(item.Count.ToString()).FontSize(9);
                            table.Cell().AlignRight().Text(item.TotalFee.ToString("N2")).FontSize(9);
                        }
                        table.Cell().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                        table.Cell().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                        table.Cell().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
                        table.Cell().Text(PdfLabels.Total).FontSize(9).Bold();
                        table.Cell().AlignRight().Text(breakdown.ExamCount.ToString()).FontSize(9).Bold();
                        table.Cell().AlignRight().Text(breakdown.TotalFee.ToString("N2")).FontSize(9).Bold().FontColor(BrandConstants.PrimaryColor);
                    });
                });
            }

            col.Item().PaddingTop(10).Background(Colors.Grey.Lighten5).Padding(15).Row(row =>
            {
                row.RelativeItem(2).Column(c => c.Item().Text("TOTAL REFERRAL FEES").FontSize(14).Bold().FontColor(BrandConstants.PrimaryColor));
                row.RelativeItem(1).Column(c => c.Item().AlignRight().Text(statement.TotalFee.ToString("N2")).FontSize(16).Bold().FontColor(BrandConstants.PrimaryColor));
            });
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
