using ConsultaTCE.Configuration;
using ConsultaTCE.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConsultaTCE.Controllers;

[ApiController]
[Route("api/resources")]
public sealed class ResourcesController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly FrontendSettings _frontendSettings;

    public ResourcesController(
        IWebHostEnvironment environment,
        IOptions<FrontendSettings> frontendOptions)
    {
        _environment = environment;
        _frontendSettings = frontendOptions.Value;
    }

    // Expoe o catalogo de recursos que o frontend pode consultar.
    [HttpGet]
    public IActionResult GetCatalog()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        return Ok(new ResourceCatalogResponse(baseUrl, GetResources()));
    }

    // Mantem a consulta paginada com o mesmo contrato ja usado pelo frontend.
    [HttpGet("{resource}")]
    public IActionResult GetResource(
        string resource,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery(Name = "codigo_municipio")] string? codigoMunicipio,
        [FromQuery] string? termo)
    {
        // Normaliza a paginacao para impedir valores invalidos vindos da UI.
        var normalizedPage = Math.Max(1, page ?? 1);
        var normalizedPageSize = Math.Clamp(pageSize ?? 25, 1, 250);
        var sourceUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        if (resource.Equals("diagnostico_backend", StringComparison.OrdinalIgnoreCase))
        {
            var item = new Dictionary<string, object?>
            {
                ["servico"] = "ConsultaTCE API",
                ["ambiente"] = _environment.EnvironmentName,
                ["porta_http"] = _frontendSettings.BackendHttpUrl,
                ["porta_https"] = _frontendSettings.BackendHttpsUrl,
                ["frontend_esperado"] = _frontendSettings.DevelopmentUrl,
                ["origens_cors"] = _frontendSettings.GetAllowedOrigins(),
                ["status"] = "Conectado",
                ["timestamp_utc"] = DateTime.UtcNow.ToString("O"),
            };

            return Ok(BuildEnvelope(resource, sourceUrl, normalizedPage, normalizedPageSize, new[] { item }));
        }

        if (resource.Equals("consulta_exemplo", StringComparison.OrdinalIgnoreCase))
        {
            // Gera uma massa previsivel para validar filtros e paginacao do frontend.
            var allItems = Enumerable.Range(1, 60)
                .Select(index => new Dictionary<string, object?>
                {
                    ["id"] = index,
                    ["municipio"] = string.IsNullOrWhiteSpace(codigoMunicipio) ? "NAO INFORMADO" : codigoMunicipio,
                    ["termo"] = string.IsNullOrWhiteSpace(termo) ? "sem filtro" : termo,
                    ["descricao"] = $"Registro de exemplo {index}",
                    ["origem"] = "API .NET",
                    ["gerado_em_utc"] = DateTime.UtcNow.ToString("O"),
                })
                .ToList();

            var filteredItems = allItems
                .Where(item =>
                {
                    if (!string.IsNullOrWhiteSpace(termo))
                    {
                        var descricao = item["descricao"]?.ToString() ?? string.Empty;
                        if (!descricao.Contains(termo, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }

                    return true;
                })
                .ToList();

            return Ok(BuildEnvelope(resource, sourceUrl, normalizedPage, normalizedPageSize, filteredItems));
        }

        return NotFound(new ProblemDetails
        {
            Title = "Recurso nao encontrado",
            Detail = $"O recurso '{resource}' nao esta disponivel nesta API.",
            Status = StatusCodes.Status404NotFound,
        });
    }

    private static IReadOnlyList<ResourceDescriptor> GetResources()
    {
        return new[]
        {
            new ResourceDescriptor(
                "diagnostico_backend",
                "/api/resources/diagnostico_backend",
                "Integracao .NET",
                "Valida a comunicacao entre o frontend Next.js e esta API .NET.",
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<QueryParameterDescriptor>()),
            new ResourceDescriptor(
                "consulta_exemplo",
                "/api/resources/consulta_exemplo",
                "Integracao .NET",
                "Retorna dados de exemplo paginados para validar a interface de consulta.",
                Array.Empty<string>(),
                new[] { "codigo_municipio", "termo" },
                new[]
                {
                    new QueryParameterDescriptor("codigo_municipio", false, "Codigo do municipio para filtrar a amostra.", "string"),
                    new QueryParameterDescriptor("termo", false, "Texto livre para demonstrar filtros dinamicos.", "string"),
                })
        };
    }

    private static PaginatedEnvelope BuildEnvelope(
        string resource,
        string sourceUrl,
        int page,
        int pageSize,
        IReadOnlyList<Dictionary<string, object?>> allItems)
    {
        // Mantem a resposta paginada padronizada para qualquer recurso exposto pela API.
        var totalItems = allItems.Count;
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = allItems
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => (IReadOnlyDictionary<string, object?>)item)
            .ToArray();
        var now = DateTime.UtcNow;

        return new PaginatedEnvelope(
            resource,
            sourceUrl,
            page,
            pageSize,
            totalItems,
            totalPages,
            items,
            new Dictionary<string, object?>
            {
                ["sourcePagination"] = false,
                ["totalItemsExact"] = true,
            },
            now,
            now.AddMinutes(5));
    }
}
