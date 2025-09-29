using App.Domain.Entities;
using App.Application.Dto;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace App.Infrastructure.Reporting
{
    public interface IPdfReportGenerator
    {
        byte[] Generate(Finding finding, List<(string id, string title, string explain, string remediation)> controls);
    }

    public class PdfReportGenerator: IPdfReportGenerator
    {
        public byte[] Generate(Finding finding, List<(string id, string title, string explain, string remediation)> controls)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Compliance Scan Report").FontSize(20).Bold();
                        col.Item().Text($"Finding: {finding.Title}").FontSize(14).Bold();
                        col.Item().Text($"Severity: {finding.Severity}");
                        col.Item().Text($"Scanner: {finding.Scanner}");
                        col.Item().Text($"Evidence: {finding.Evidence}").FontSize(10);

                        col.Item().Text("\nMapped HIPAA Controls:").Bold();
                        foreach (var c in controls)
                        {
                            col.Item().Text($"{c.id} - {c.title}").Bold();
                            col.Item().Text($"{c.explain}");
                            col.Item().Text($"Remediation: {c.remediation}\n");
                        }
                    });
                });
            });

            using var ms = new MemoryStream();
            doc.GeneratePdf(ms);
            return ms.ToArray();
        }
    }
}


