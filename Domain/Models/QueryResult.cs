namespace Domain.Models;

// Envelope padrao devolvido ao front-end apos uma consulta ao TCE-CE.
public class QueryResult
{
    // Endpoint selecionado pelo usuario.
    public string EndpointKey { get; init; } = string.Empty;

    // Caminho remoto efetivamente consumido.
    public string EndpointPath { get; init; } = string.Empty;

    // Municipio usado como referencia da busca.
    public string MunicipalityCode { get; init; } = string.Empty;

    // Nome amigavel do municipio para a tela.
    public string MunicipalityName { get; init; } = string.Empty;

    // URL remota completa usada na consulta.
    public string SourceUrl { get; init; } = string.Empty;

    // Colunas inferidas a partir das chaves presentes no retorno.
    public IReadOnlyList<string> Columns { get; init; } = [];

    // Linhas da grade em formato generico para suportar qualquer endpoint.
    public IReadOnlyList<IReadOnlyDictionary<string, string>> Items { get; init; } = [];

    // Metadados complementares para pagina, cache e comportamento do endpoint.
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    // Informacoes de paginacao para controlar a grade no front-end.
    public QueryPagination Pagination { get; init; } = new();
}

// Estrutura isolada de paginacao para manter o contrato HTTP mais claro.
public class QueryPagination
{
    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalItems { get; init; }

    public int TotalPages { get; init; }

    public bool HasMorePages { get; init; }
}

// Opcao de municipio carregada no seletor da tela.
public class MunicipalityOption
{
    // Codigo exigido pela API publica.
    public string Code { get; init; } = string.Empty;

    // Nome exibido para o usuario.
    public string Name { get; init; } = string.Empty;
}
