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
            return BadRequest(new { mensagem = "Arquivo não enviado ou vazio." });

        try
        {
            using var stream = arquivo.OpenReadStream();

            // O backend passa a inferir a competencia pela Referencia de cada linha.
            // Isso remove a dependencia do seletor manual sem mudar a gravacao da Referencia na tabela.
            var registrosImportados = await _contratoService.ImportarContratosAsync(stream);
            return Ok(new { mensagem = $"{registrosImportados} registros importados com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
    
    [HttpGet("buscar-por-contrato")] // Rota fixa, sem o {parametro}
    public async Task<ActionResult<ContratoPagedResultDTO>> GetByContrato(
        [FromQuery] string? numeroContrato,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // A grade navega sempre em lotes de até 20 registros.
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize switch
        {
            <= 0 => 20,
            > 20 => 20,
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
