using App.Api.Models;
using App.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HipaaRiskController : ControllerBase
    {
        private readonly IHipaaRiskService _svc;
        public HipaaRiskController(IHipaaRiskService svc) => _svc = svc;

        [HttpPost("hipaa")]
        public async Task<IActionResult> Analyze([FromBody] HipaaRiskRequest request, CancellationToken ct)
        {
            try
            {
                var res = await _svc.AnalyzeRepoAsync(request, ct);
                return Ok(res);
            }
            catch (Exception ex)
            {
                // don't leak sensitive details to callers: log server-side and return safe message
                // TODO: integrate ILogger to log full exception
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
