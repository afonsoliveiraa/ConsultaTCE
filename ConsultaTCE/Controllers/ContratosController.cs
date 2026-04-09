using Application.Services;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaTCE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContratosController : ControllerBase
{
    private readonly ContratoService _contratoService;

    public ContratosController(ContratoService contratoService)
    {
        _contratoService = contratoService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest("Arquivo não enviado ou vazio.");

        // Abrimos o stream do arquivo enviado pelo usuário
        using var stream = arquivo.OpenReadStream();
        
        // Chamamos o serviço que orquestra todo o processo DDD
        await _contratoService.ImportarContratosAsync(stream);

        return Ok(new { mensagem = "Contratos processados e salvos com sucesso!" });
    }
    
    [HttpGet("buscar-por-contrato")] // Rota fixa, sem o {parametro}
    public async Task<ActionResult<ContratoPagedResultDTO>> GetByContrato(
        [FromQuery] string? numeroContrato,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        // A grade navega sempre em lotes de até 50 registros.
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize switch
        {
            <= 0 => 50,
            > 50 => 50,
            _ => pageSize
        };

        var resultado = await _contratoService.BuscarPorNumeroContratoAsync(
            numeroContrato,
            normalizedPage,
            normalizedPageSize);

        if (resultado.TotalItems == 0)
        {
            return string.IsNullOrWhiteSpace(numeroContrato)
                ? NotFound("Nenhum contrato encontrado na base.")
                : NotFound($"Nenhum contrato encontrada para seguinte número: {numeroContrato}");
        }

        return Ok(resultado);
    }
}
