namespace Domain.Models;

// Representa a intencao de consulta enviada pelo front-end para a API interna.
public class QueryRequest
{
    // Codigo do municipio selecionado na tela.
    public string MunicipalityCode { get; init; } = string.Empty;

    // Nome do municipio apenas para exibir no retorno e facilitar debug.
    public string MunicipalityName { get; init; } = string.Empty;

    // Endpoint escolhido pelo usuario dentro do catalogo carregado na pagina.
    public string EndpointKey { get; init; } = string.Empty;

    // Parametros dinamicos do formulario.
    public IReadOnlyDictionary<string, string> Parameters { get; init; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    // Pagina solicitada pelo front-end.
    public int Page { get; init; } = 1;

    // Quantidade de itens por pagina.
    public int PageSize { get; init; } = 20;

    // Normaliza a pagina para evitar numeros invalidos.
    public int NormalizedPage => Page < 1 ? 1 : Page;

    // Normaliza o tamanho da pagina para impedir consultas excessivas.
    public int NormalizedPageSize => PageSize switch
    {
        < 1 => 20,
        > 100 => 100,
        _ => PageSize
    };
}
