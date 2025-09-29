using App.Domain.Entities;
using App.Application.Dto;
using QuestPDF.Fluent;

namespace App.Infrastructure.Reporting
{
    public class PdfReportGenerator
    {
        public byte[] GenerateReport(Finding finding, List<HippaControlDto> controls)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Header().Text("Security Scan Report").FontSize(20).Bold();
                    page.Content().Padding(10).Column(col =>
                    {
                        col.Item().Text($"Finding: {finding.Title}");
                        col.Item().Text($"Description: {finding.Description}");
                        col.Item().Text($"Evidence: {finding.Evidence}");

                        col.Item().Text("Mapped HIPAA Controls:");
                        foreach (var ctrl in controls)
                        {
                            col.Item().Text($"- {ctrl.Id}: {ctrl.Title}");
                            col.Item().Text($"  {ctrl.Explain}");
                            col.Item().Text($"  Remediation: {ctrl.Remediation}");
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}


