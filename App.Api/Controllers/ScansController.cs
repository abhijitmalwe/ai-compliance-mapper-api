using App.Application.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScansController : ControllerBase
    {
        private readonly IRepoScannerOrchestrator _orchestrator;
        public ScansController(IRepoScannerOrchestrator orchestrator) => _orchestrator = orchestrator;

        //[HttpPost("scan")]
        //public async Task<IActionResult> StartScan([FromBody] ScanRequestDto dto, CancellationToken ct)
        //{
        //    if (string.IsNullOrWhiteSpace(dto.CloneUrl))
        //        return BadRequest("cloneUrl required");

        //    var result = await _orchestrator.RunFullScanAsync(dto.CloneUrl, dto.Branch ?? "main", ct);
        //    return Ok(result); // the orchestrator returns summary + path to PDF or base64 PDF bytes
        //}
    }
}
