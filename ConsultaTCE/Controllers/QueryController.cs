using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaTCE.Controllers;

// Recebe a intencao de consulta do front-end e devolve um grid generico.
[ApiController]
[Route("api/tce")]
public class QueryController : ControllerBase
{
    private readonly TceAppService _tceAppService;

    public QueryController(TceAppService tceAppService)
    {
        _tceAppService = tceAppService;
    }

    // Executa a consulta dinamica em qualquer endpoint do catalogo.
    [HttpPost("query")]
    public async Task<IActionResult> Query(
        [FromBody] QueryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _tceAppService.QueryAsync(request, cancellationToken);
        return Ok(result);
    }
}
