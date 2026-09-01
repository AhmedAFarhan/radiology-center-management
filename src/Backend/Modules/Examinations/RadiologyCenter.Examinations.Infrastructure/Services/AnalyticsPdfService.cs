using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Reports;

namespace RadiologyCenter.Examinations.Infrastructure.Services;

public sealed class AnalyticsPdfService : IAnalyticsPdfService
{
    private const string PrimaryColor = "#4C58E0";

    ReportContentDto IAnalyticsPdfService.BuildFinancialPdf(FinancialAnalyticsDto data, DateTime from, DateTime to)
        => BuildFinancialPdf(data, from, to);

    ReportContentDto IAnalyticsPdfService.BuildOperationalPdf(OperationalAnalyticsDto data, DateTime from, DateTime to)
        => BuildOperationalPdf(data, from, to);

    ReportContentDto IAnalyticsPdfService.BuildStaffMachinePdf(StaffMachineAnalyticsDto data, DateTime from, DateTime to)
        => BuildStaffMachinePdf(data, from, to);

    ReportContentDto IAnalyticsPdfService.BuildProfitPdf(ProfitAnalyticsDto data, DateTime from, DateTime to)
        => BuildProfitPdf(data, from, to);

    ReportContentDto IAnalyticsPdfService.BuildInsurancePdf(InsuranceAnalyticsDto data, DateTime from, DateTime to)
        => BuildInsurancePdf(data, from, to);

    ReportContentDto IAnalyticsPdfService.BuildCashFlowPdf(CashFlowReportDto data, DateTime from, DateTime to)
        => BuildCashFlowPdf(data, from, to);

    public ReportContentDto BuildFinancialPdf(FinancialAnalyticsDto data, DateTime from, DateTime to)
    {
        var bytes = BuildPdf(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.MarginHorizontal(30);
                page.MarginVertical(20);
                page.Header().Element(h => BuildHeader(h, "Financial Report", from, to));
                page.Content().Element(c =>
                {
                    c.PaddingTop(10).Column(col =>
                    {
                        col.Item().Text($"Total Examinations: {data.ExamCount}").FontSize(10);
                        col.Item().Text($"Revenue Collected: {data.TotalCollected:N2}").FontSize(10);
                        col.Item().Text($"Total Billed: {data.TotalBilled:N2}").FontSize(10);
                        col.Item().Text($"Total Discounts: {data.TotalDiscounts:N2}").FontSize(10);
                        col.Item().Text($"Outstanding Receivables: {data.Receivables:N2}").FontSize(10);
                        col.Item().Text($"Average Per Exam: {data.AvgPerExam:N2}").FontSize(10);

                        col.Item().PaddingTop(15).Text("Revenue by Month").FontSize(11).Bold().FontColor(PrimaryColor);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Month").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Collected").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Billed").FontSize(8).Bold();
                            });
                            foreach (var row in data.RevenueByMonth)
                            {
                                table.Cell().Text(row.Month).FontSize(9);
                                table.Cell().AlignRight().Text(row.Collected.ToString("N2")).FontSize(9);
                                table.Cell().AlignRight().Text(row.Billed.ToString("N2")).FontSize(9);
                            }
                        });

                        col.Item().PaddingTop(15).Text("Revenue by Modality").FontSize(11).Bold().FontColor(PrimaryColor);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Modality").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Collected").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Exam Count").FontSize(8).Bold();
                            });
                            foreach (var row in data.RevenueByModality)
                            {
                                table.Cell().Text(row.Modality).FontSize(9);
                                table.Cell().AlignRight().Text(row.Collected.ToString("N2")).FontSize(9);
                                table.Cell().AlignRight().Text(row.ExamCount.ToString()).FontSize(9);
                            }
                        });

                        col.Item().PaddingTop(15).Text("Receivable Aging").FontSize(11).Bold().FontColor(PrimaryColor);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Bucket").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Amount").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Count").FontSize(8).Bold();
                            });
                            foreach (var row in data.ReceivableAging)
                            {
                                table.Cell().Text(row.Bucket).FontSize(9);
                                table.Cell().AlignRight().Text(row.Amount.ToString("N2")).FontSize(9);
                                table.Cell().AlignRight().Text(row.ExamCount.ToString()).FontSize(9);
                            }
                        });
                    });
                });
                page.Footer().Element(BuildFooter);
            });
        });
        return new ReportContentDto(bytes, $"FinancialReport_{from:yyyyMMdd}-{to:yyyyMMdd}.pdf", "application/pdf");
    }

    public ReportContentDto BuildOperationalPdf(OperationalAnalyticsDto data, DateTime from, DateTime to)
    {
        var bytes = BuildPdf(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.MarginHorizontal(30);
                page.MarginVertical(20);
                page.Header().Element(h => BuildHeader(h, "Operational Report", from, to));
                page.Content().Element(c =>
                {
                    c.PaddingTop(10).Column(col =>
                    {
                        col.Item().Text($"Total Examinations: {data.TotalExams}").FontSize(10);
                        col.Item().Text($"Completed: {data.CompletedExams}").FontSize(10);
                        col.Item().Text($"Cancelled: {data.CancelledExams}").FontSize(10);
                        col.Item().Text($"Completion Rate: {data.CompletionRate:P1}").FontSize(10);
                        col.Item().Text($"Avg Duration: {data.AvgDurationMinutes:F0} minutes").FontSize(10);
                        col.Item().Text($"Avg Time to Start: {data.AvgTimeToStartMinutes:F0} minutes").FontSize(10);

                        col.Item().PaddingTop(15).Text("Volume by Month").FontSize(11).Bold().FontColor(PrimaryColor);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Month").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Total").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Completed").FontSize(8).Bold();
                            });
                            foreach (var row in data.VolumeByMonth)
                            {
                                table.Cell().Text(row.Month).FontSize(9);
                                table.Cell().AlignRight().Text(row.Total.ToString()).FontSize(9);
                                table.Cell().AlignRight().Text(row.Completed.ToString()).FontSize(9);
                            }
                        });
                    });
                });
                page.Footer().Element(BuildFooter);
            });
        });
        return new ReportContentDto(bytes, $"OperationalReport_{from:yyyyMMdd}-{to:yyyyMMdd}.pdf", "application/pdf");
    }

    public ReportContentDto BuildStaffMachinePdf(StaffMachineAnalyticsDto data, DateTime from, DateTime to)
    {
        var bytes = BuildPdf(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.MarginHorizontal(30);
                page.MarginVertical(20);
                page.Header().Element(h => BuildHeader(h, "Staff & Machine Report", from, to));
                page.Content().Element(c =>
                {
                    c.PaddingTop(10).Column(col =>
                    {
                        col.Item().Text("Radiologists").FontSize(11).Bold().FontColor(PrimaryColor);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Name").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Exams").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Fee Income").FontSize(8).Bold();
                            });
                            foreach (var s in data.Radiologists)
                            {
                                table.Cell().Text(s.Name).FontSize(9);
                                table.Cell().AlignRight().Text(s.CompletedExams.ToString()).FontSize(9);
                                table.Cell().AlignRight().Text(s.FeeIncome.ToString("N2")).FontSize(9);
                            }
                        });

                        col.Item().PaddingTop(15).Text("Technicians").FontSize(11).Bold().FontColor(PrimaryColor);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Name").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Exams").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Fee Income").FontSize(8).Bold();
                            });
                            foreach (var s in data.Technicians)
                            {
                                table.Cell().Text(s.Name).FontSize(9);
                                table.Cell().AlignRight().Text(s.CompletedExams.ToString()).FontSize(9);
                                table.Cell().AlignRight().Text(s.FeeIncome.ToString("N2")).FontSize(9);
                            }
                        });

                        col.Item().PaddingTop(15).Text("Referral Doctors").FontSize(11).Bold().FontColor(PrimaryColor);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Name").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Referred").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Fee Income").FontSize(8).Bold();
                            });
                            foreach (var r in data.ReferralDoctors)
                            {
                                table.Cell().Text(r.Name).FontSize(9);
                                table.Cell().AlignRight().Text(r.ReferredExams.ToString()).FontSize(9);
                                table.Cell().AlignRight().Text(r.ReferralFeeIncome.ToString("N2")).FontSize(9);
                            }
                        });
                    });
                });
                page.Footer().Element(BuildFooter);
            });
        });
        return new ReportContentDto(bytes, $"StaffReport_{from:yyyyMMdd}-{to:yyyyMMdd}.pdf", "application/pdf");
    }

    public ReportContentDto BuildProfitPdf(ProfitAnalyticsDto data, DateTime from, DateTime to)
    {
        var bytes = BuildPdf(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(30);
                page.MarginVertical(20);
                page.Header().Element(h => BuildHeader(h, "Profit Report", from, to));
                page.Content().Element(c =>
                {
                    c.PaddingTop(10).Column(col =>
                    {
                        col.Item().Text("REVENUE").FontSize(11).Bold().FontColor(Colors.Green.Darken1);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Item").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Amount").FontSize(8).Bold();
                            });
                            table.Cell().Text("Revenue Collected").FontSize(9);
                            table.Cell().AlignRight().Text(data.RevenueCollected.ToString("N2")).FontSize(9);
                            table.Cell().Text("Total Billed").FontSize(9);
                            table.Cell().AlignRight().Text(data.TotalBilled.ToString("N2")).FontSize(9);
                            table.Cell().Text("Discounts").FontSize(9);
                            table.Cell().AlignRight().Text(data.Discounts.ToString("N2")).FontSize(9);
                        });

                        col.Item().PaddingTop(15).Text("COSTS").FontSize(11).Bold().FontColor(Colors.Red.Darken1);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Item").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Amount").FontSize(8).Bold();
                            });
                            table.Cell().Text("Staff Case Fees").FontSize(9);
                            table.Cell().AlignRight().Text(data.StaffCaseFees.ToString("N2")).FontSize(9);
                            table.Cell().Text("Referral Fees").FontSize(9);
                            table.Cell().AlignRight().Text(data.ReferralFees.ToString("N2")).FontSize(9);
                            table.Cell().Text("Labor Costs").FontSize(9);
                            table.Cell().AlignRight().Text(data.LaborCosts.ToString("N2")).FontSize(9);
                            table.Cell().Text("Material Costs").FontSize(9);
                            table.Cell().AlignRight().Text(data.MaterialCosts.ToString("N2")).FontSize(9);
                            table.Cell().PaddingTop(3).LineHorizontal(0.5f);
                            table.Cell().PaddingTop(3).LineHorizontal(0.5f);
                            table.Cell().Text("Total Costs").FontSize(9).Bold();
                            table.Cell().AlignRight().Text(data.TotalCosts.ToString("N2")).FontSize(9).Bold();
                        });

                        col.Item().PaddingTop(15).Background(Colors.Grey.Lighten5).Padding(15).Row(row =>
                        {
                            row.RelativeItem(2).Column(c => c.Item().Text("NET PROFIT").FontSize(14).Bold().FontColor(PrimaryColor));
                            row.RelativeItem(1).Column(c => c.Item().AlignRight().Text(data.NetProfit.ToString("N2")).FontSize(16).Bold().FontColor(PrimaryColor));
                        });

                        col.Item().PaddingTop(5).AlignCenter().Text($"Margin: {data.NetMargin:P1}").FontSize(10).FontColor(Colors.Grey.Darken1);
                    });
                });
                page.Footer().Element(BuildFooter);
            });
        });
        return new ReportContentDto(bytes, $"ProfitReport_{from:yyyyMMdd}-{to:yyyyMMdd}.pdf", "application/pdf");
    }

    public ReportContentDto BuildInsurancePdf(InsuranceAnalyticsDto data, DateTime from, DateTime to)
    {
        var bytes = BuildPdf(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.MarginHorizontal(30);
                page.MarginVertical(20);
                page.Header().Element(h => BuildHeader(h, "Insurance Claims Report", from, to));
                page.Content().Element(c =>
                {
                    c.PaddingTop(10).Column(col =>
                    {
                        col.Item().Text($"Total Claims: {data.TotalClaims}").FontSize(10);
                        col.Item().Text($"Approved: {data.ApprovedClaims}  |  Rejected: {data.RejectedClaims}  |  Paid: {data.PaidClaims}").FontSize(10);
                        col.Item().Text($"Total Billed: {data.TotalBilledAmount:N2}").FontSize(10);
                        col.Item().Text($"Total Settled: {data.TotalSettled:N2}").FontSize(10);
                        col.Item().Text($"Outstanding: {data.OutstandingAmount:N2}").FontSize(10);
                        col.Item().Text($"Approval Rate: {data.ApprovalRate:P1}").FontSize(10);

                        col.Item().PaddingTop(15).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Patient").FontSize(8).Bold();
                                h.Cell().Text("Company").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Billed").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Payer").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Patient").FontSize(8).Bold();
                                h.Cell().Text("Status").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Remaining").FontSize(8).Bold();
                            });
                            foreach (var row in data.ClaimRows)
                            {
                                table.Cell().Text(row.PatientName).FontSize(8);
                                table.Cell().Text(row.InsuranceCompany).FontSize(8);
                                table.Cell().AlignRight().Text(row.BilledAmount.ToString("N2")).FontSize(8);
                                table.Cell().AlignRight().Text(row.PayerShare.ToString("N2")).FontSize(8);
                                table.Cell().AlignRight().Text(row.PatientShare.ToString("N2")).FontSize(8);
                                table.Cell().Text(row.Status).FontSize(8);
                                table.Cell().AlignRight().Text(row.RemainingOwed.ToString("N2")).FontSize(8);
                            }
                        });
                    });
                });
                page.Footer().Element(BuildFooter);
            });
        });
        return new ReportContentDto(bytes, $"InsuranceReport_{from:yyyyMMdd}-{to:yyyyMMdd}.pdf", "application/pdf");
    }

    public ReportContentDto BuildCashFlowPdf(CashFlowReportDto data, DateTime from, DateTime to)
    {
        var bytes = BuildPdf(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.MarginHorizontal(30);
                page.MarginVertical(20);
                page.Header().Element(h => BuildHeader(h, "Cash Flow Report", from, to));
                page.Content().Element(c =>
                {
                    c.PaddingTop(10).Column(col =>
                    {
                        col.Item().Text($"Total Inflows: {data.TotalInflows:N2}").FontSize(10);
                        col.Item().Text($"Total Outflows: {data.TotalOutflows:N2}").FontSize(10);
                        col.Item().Text($"Net Cash Flow: {data.NetCashFlow:N2}").FontSize(10);
                        col.Item().Text($"Total Sessions: {data.TotalSessions}").FontSize(10);
                        col.Item().Text($"Avg Session Balance: {data.AvgSessionBalance:N2}").FontSize(10);

                        col.Item().PaddingTop(15).Text("Monthly Breakdown").FontSize(11).Bold().FontColor(PrimaryColor);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Month").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Inflows").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Outflows").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Net").FontSize(8).Bold();
                            });
                            foreach (var row in data.ByMonth)
                            {
                                table.Cell().Text(row.Month).FontSize(9);
                                table.Cell().AlignRight().Text(row.Inflows.ToString("N2")).FontSize(9);
                                table.Cell().AlignRight().Text(row.Outflows.ToString("N2")).FontSize(9);
                                table.Cell().AlignRight().Text(row.Net.ToString("N2")).FontSize(9);
                            }
                        });

                        col.Item().PaddingTop(15).Text("By Entry Reason").FontSize(11).Bold().FontColor(PrimaryColor);
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Reason").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Inflows").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Outflows").FontSize(8).Bold();
                                h.Cell().AlignRight().Text("Count").FontSize(8).Bold();
                            });
                            foreach (var row in data.ByReason)
                            {
                                table.Cell().Text(row.Reason).FontSize(9);
                                table.Cell().AlignRight().Text(row.InflowAmount.ToString("N2")).FontSize(9);
                                table.Cell().AlignRight().Text(row.OutflowAmount.ToString("N2")).FontSize(9);
                                table.Cell().AlignRight().Text(row.EntryCount.ToString()).FontSize(9);
                            }
                        });
                    });
                });
                page.Footer().Element(BuildFooter);
            });
        });
        return new ReportContentDto(bytes, $"CashFlowReport_{from:yyyyMMdd}-{to:yyyyMMdd}.pdf", "application/pdf");
    }

    private static byte[] BuildPdf(Action<IDocumentContainer> content)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var document = Document.Create(content);

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    private static void BuildHeader(IContainer container, string title, DateTime from, DateTime to)
    {
        container.Row(row =>
        {
            row.RelativeItem().Width(50).Column(col =>
            {
                var assembly = typeof(AnalyticsPdfService).Assembly;
                using var stream = assembly.GetManifestResourceStream("RadiologyCenter.Examinations.Infrastructure.Resources.logo.png");
                if (stream is not null)
                {
                    using var ms = new MemoryStream();
                    stream.CopyTo(ms);
                    col.Item().Image(ms.ToArray());
                }
            });

            row.RelativeItem(3).Column(col =>
            {
                col.Item().Text("RADIOLOGY CENTER").FontSize(18).Bold().FontColor(PrimaryColor);
                col.Item().Text(title.ToUpperInvariant()).FontSize(14).Bold().FontColor(Colors.Grey.Darken1);
                col.Item().PaddingTop(5).Text($"Period: {from:MMM dd, yyyy} - {to:MMM dd, yyyy}").FontSize(9).FontColor(Colors.Grey.Medium);
            });
        });
    }

    private static void BuildFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("Generated on ").FontSize(8).FontColor(Colors.Grey.Medium);
            text.Span(DateTime.Now.ToString("MMM dd, yyyy HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
            text.Span(" | This is a system-generated document.").FontSize(8).FontColor(Colors.Grey.Medium);
        });
    }
}
