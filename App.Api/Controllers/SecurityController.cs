using App.Application.Dto;
using App.Application.Interfaces;
using App.Domain.Entities;
using App.Infrastructure.Repo;
using App.Infrastructure.Reporting;
using App.Infrastructure.Scanners;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly RepoService _repoService;
        private readonly ZapScanner _scanner;
        private readonly ISecurityMappingService _mapping;
        private readonly IVectorService _vector;
        private readonly IGitHubIssueService _issueService;
        private readonly IPdfReportGenerator _pdfGen;


        public SecurityController(
             RepoService repoService,
             ZapScanner scanner,
             ISecurityMappingService mapping,
             IVectorService vector,
             IGitHubIssueService issueService,
             IPdfReportGenerator pdfGen)
        {
            _repoService = repoService;
            _scanner = scanner;
            _mapping = mapping;
            _vector = vector;
            _issueService = issueService;
            _pdfGen = pdfGen;
        }

        [HttpPost("scanRepo")]
        public async Task<IActionResult> ScanRepo([FromBody] ScanRequestDto request)
        {
            // 1. Clone repo
            string repoPath;
            try
            {
                repoPath = _repoService.CloneRepo(request.CloneUrl, request.Branch ?? "main", request.Token ?? null);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = "Clone failed", message = ex.Message });
            }

            // 2. Run scanner (simulated)
            List<Finding> findings = _scanner.Scan(repoPath);

            // For simplicity, handle first finding
            var finding = findings.Count > 0 ? findings[0] : new Finding { Title = "No issues", Description = "No findings", Evidence = "" };

            // 3. Map HIPAA
            var controls = _mapping.MapFindingToHipaaControls(finding);

            // 4. Store in Qdrant
            await _vector.StoreAsync(finding.Id, finding.Description);

            // 5. Create GitHub issue (if owner/repo provided in request)
            if (!string.IsNullOrEmpty(request.RepoOwner) && !string.IsNullOrEmpty(request.RepoName))
            {
                var body = $"Automated compliance finding:\n\n{finding.Title}\n\n{finding.Description}\n\nEvidence:\n{finding.Evidence}";
                await _issueService.CreateIssueAsync(request.RepoOwner, request.RepoName, finding.Title, body);
            }

            // 6. Generate PDF
            var pdfBytes = _pdfGen.Generate(finding, controls);

            // return PDF file
            return File(pdfBytes, "application/pdf", "scan-report.pdf");
        }
    }
}
