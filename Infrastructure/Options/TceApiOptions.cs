using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

// Centraliza as configuracoes da integracao com a API publica do TCE-CE.
public class TceApiOptions
{
    public const string SectionName = "TceCeApi";

    // URL base usada para montar as requisicoes remotas.
    [Required]
    [Url]
    public string BaseUrl { get; init; } = "https://api-dados-abertos.tce.ce.gov.br/";

    // Tempo de vida do cache em segundos.
    [Range(30, 86400)]
    public int CacheSeconds { get; init; } = 300;

    // Header opcional para ambientes que exigirem autenticacao.
    public string ApiKeyHeaderName { get; init; } = string.Empty;

    // Valor do header opcional.
    public string ApiKey { get; init; } = string.Empty;

    // Caminho do arquivo que contem o swagger embutido do portal.
    public string SwaggerUiInitPath { get; init; } = @"C:\Users\joaov\API-TCE-CE\swagger-ui-init.js";

    // URL da pagina oficial de documentacao usada para descobrir o swagger publicado pelo tribunal.
    public string DocsUrl { get; init; } = "https://api-dados-abertos.tce.ce.gov.br/docs/";

    // URLs opcionais para descoberta remota do catalogo publicada pelo tribunal.
    public string[] SwaggerDocumentUrls { get; init; } = [];

    public string[] SwaggerUiInitUrls { get; init; } = [];

    // Fallback de endpoints caso a leitura do swagger nao esteja disponivel.
    public Dictionary<string, TceEndpointOption> Resources { get; init; } =
        new(StringComparer.OrdinalIgnoreCase);
}

// Estrutura de configuracao de um endpoint individual.
public class TceEndpointOption
{
    public string Path { get; init; } = string.Empty;

    public string Category { get; init; } = "Outros";

    public string Description { get; init; } = string.Empty;

    public string[] RequiredQueryParameters { get; init; } = [];

    public string[] OptionalQueryParameters { get; init; } = [];
}
