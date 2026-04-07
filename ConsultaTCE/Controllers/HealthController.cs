using Microsoft.AspNetCore.Mvc;

namespace ConsultaTCE.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    // Endpoint simples para diagnostico do host e da integracao com o frontend.
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            service = "ConsultaTCE",
            timestampUtc = DateTime.UtcNow,
        });
    }
}
