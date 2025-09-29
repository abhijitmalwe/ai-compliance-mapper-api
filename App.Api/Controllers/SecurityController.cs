using App.Application.Dto;
using App.Application.Interfaces;
using App.Domain.Entities;
using App.Infrastructure.Repo;
using App.Infrastructure.Reporting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityMappingService _securityMapping;
        private readonly IVectorService _vectorService;
        private readonly IGitHubIssueService _gitHubService;
        private readonly RepoService _repoService;

        public SecurityController(
            ISecurityMappingService securityMapping,
            IVectorService vectorService,
            IGitHubIssueService gitHubService,
            RepoService repoService)
        {
            _securityMapping = securityMapping;
            _vectorService = vectorService;
            _gitHubService = gitHubService;
            _repoService = repoService;
        }

        [HttpPost("scanRepo")]
        public async Task<IActionResult> ScanRepo([FromBody] ScanRequestDto request)
        {
            // 1. Clone repo
            var repoPath = _repoService.CloneRepo(request.CloneUrl, request.Branch);

            // 2. Simulated finding
            var finding = new Finding
            {
                Title = "Hardcoded DB password",
                Description = "Detected hardcoded password in appsettings.json",
                Evidence = "\"ConnectionStrings:Default\": \"Server=...;Password=pa$$w0rd;\""
            };

            // 3. Map HIPAA controls
            var controls = _securityMapping.MapFindingToHipaaControls(finding.Description);

            // 4. Store in Qdrant
            await _vectorService.StoreEmbeddingAsync(finding.Id, finding.Description);

            // 5. Create GitHub issue
            await _gitHubService.CreateIssueAsync("owner", "repo", finding.Title, finding.Description);

            // 6. Generate PDF
            var pdfGen = new PdfReportGenerator();
            var pdfBytes = pdfGen.GenerateReport(finding, controls);

            return File(pdfBytes, "application/pdf", "scan-report.pdf");
        }
    }
}
