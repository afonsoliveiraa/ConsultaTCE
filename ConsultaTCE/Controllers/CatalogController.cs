using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaTCE.Controllers;

// Exponibiliza os dados fixos e de apoio usados para montar a tela "API TCE".
[ApiController]
[Route("api/catalog/tce")]
public class CatalogController : ControllerBase
{
    private readonly TceAppService _tceAppService;

    public CatalogController(TceAppService tceAppService)
    {
        _tceAppService = tceAppService;
    }

    // Retorna o catalogo completo de endpoints exibido no seletor da pagina.
    [HttpGet("endpoints")]
    public async Task<IActionResult> GetEndpoints(CancellationToken cancellationToken)
    {
        var endpoints = await _tceAppService.GetCatalogAsync(cancellationToken);
        return Ok(endpoints);
    }

    // Retorna os municipios que alimentam o primeiro card da tela.
    [HttpGet("municipios")]
    public async Task<IActionResult> GetMunicipalities(CancellationToken cancellationToken)
    {
        var municipalities = await _tceAppService.GetMunicipalitiesAsync(cancellationToken);
        return Ok(municipalities);
    }
}
