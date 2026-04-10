using Application.Services;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaTCE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LicitacoesController : ControllerBase
{
    private readonly LicitacaoService _licitacaoService;

    public LicitacoesController(LicitacaoService licitacaoService)
    {
        _licitacaoService = licitacaoService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")] // Adicione esta linha aqui
    public async Task<IActionResult> Upload(IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest(new { mensagem = "Arquivo não enviado ou vazio." });

        try
        {
            using var stream = arquivo.OpenReadStream();

            // O backend infere o layout (2017-2026) automaticamente pela referência no arquivo.
            var registrosImportados = await _licitacaoService.ImportarLicitacoesAsync(stream);
            
            return Ok(new { mensagem = $"{registrosImportados} licitações importadas com sucesso." });
        }
        catch (Exception ex)
        {
            // Captura erros de layout ou parsing mapeados no LeitorLI
            return BadRequest(new { mensagem = ex.Message });
        }
    }
    
    [HttpGet("buscar-por-processo")]
    public async Task<ActionResult<LicitacaoPagedResultDTO>> GetByProcesso(
        [FromQuery] string? numeroProcesso,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // Normalização de paginação seguindo o padrão do projeto
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize switch
        {
            <= 0 => 20,
            > 20 => 20,
            _ => pageSize
        };

        var resultado = await _licitacaoService.BuscarPorNumeroProcessoAsync(
            numeroProcesso,
            normalizedPage,
            normalizedPageSize);

        if (resultado.TotalItems == 0)
        {
            return string.IsNullOrWhiteSpace(numeroProcesso)
                ? NotFound(new { mensagem = "Nenhuma licitação encontrada na base." })
                : NotFound(new { mensagem = $"Nenhuma licitação encontrada para o processo: {numeroProcesso}" });
        }

        return Ok(resultado);
    }
}