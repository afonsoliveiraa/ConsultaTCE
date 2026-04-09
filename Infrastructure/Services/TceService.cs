using System.Text.Json;
using Domain.Entities;
using Domain.Models;
using Infrastructure.Http;
using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

// Implementa a integracao do catalogo e das consultas do TCE-CE reaproveitando
// a estrutura DDD da solucao atual.
public class TceService
{
    private static readonly HashSet<string> HiddenRequiredParameters =
        new(["codigo_municipio", "quantidade", "deslocamento"], StringComparer.OrdinalIgnoreCase);

    private readonly TceHttpClient _httpClient;
    private readonly ILogger<TceService> _logger;
    private readonly IReadOnlyDictionary<string, Endpoint> _endpoints;

    public TceService(
        TceHttpClient httpClient,
        IOptions<TceApiOptions> options,
        ILogger<TceService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _endpoints = LoadEndpoints(options.Value);
    }

    // Entrega a lista de endpoints que o front-end usa para preencher o seletor.
    public Task<IReadOnlyList<Endpoint>> GetEndpointsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Endpoint>>(_endpoints.Values.ToArray());
    }

    // Consulta o endpoint de municipios para o usuario escolher o contexto da busca.
    public async Task<IReadOnlyList<MunicipalityOption>> GetMunicipalitiesAsync(CancellationToken cancellationToken)
    {
        var endpoint = GetEndpoint("municipios");
        var response = await _httpClient.GetAsync(endpoint.Path, new Dictionary<string, string>(), cancellationToken);

        var municipalities = response.Items
            .Select(item => new MunicipalityOption
            {
                Code = ReadFirst(item, "codigo_municipio", "codigoMunicipio"),
                Name = ReadFirst(item, "nome_municipio", "nomeMunicipio")
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.Code) && !string.IsNullOrWhiteSpace(item.Name))
            .OrderBy(item => item.Name)
            .ToArray();

        return municipalities;
    }

    // Executa a consulta dinamica montando a query string exigida pela API publica.
    public async Task<QueryResult> QueryAsync(QueryRequest request, CancellationToken cancellationToken)
    {
        var endpoint = GetEndpoint(request.EndpointKey);
        var queryParameters = BuildQueryParameters(endpoint, request);

        var response = await _httpClient.GetAsync(endpoint.Path, queryParameters, cancellationToken);
        var paginatedItems = PaginateItems(endpoint, request, response.Items, out var totalItems, out var totalPages, out var hasMorePages);
        var columns = InferColumns(paginatedItems);

        _logger.LogInformation(
            "Consulta TCE executada. Endpoint: {Endpoint}. Municipio: {Municipio}. ItensRetornados: {Itens}. Pagina: {Pagina}.",
            endpoint.Key,
            request.MunicipalityCode,
            paginatedItems.Count,
            request.NormalizedPage);

        return new QueryResult
        {
            EndpointKey = endpoint.Key,
            EndpointPath = endpoint.Path,
            MunicipalityCode = request.MunicipalityCode,
            MunicipalityName = request.MunicipalityName,
            SourceUrl = response.SourceUrl,
            Columns = columns,
            Items = paginatedItems,
            Metadata = response.Metadata,
            Pagination = new QueryPagination
            {
                Page = request.NormalizedPage,
                PageSize = request.NormalizedPageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasMorePages = hasMorePages
            }
        };
    }

    // Carrega o catalogo primeiro do swagger do portal; se falhar, usa o fallback do appsettings.
    private static IReadOnlyDictionary<string, Endpoint> LoadEndpoints(TceApiOptions options)
    {
        var swaggerCatalog = TryLoadEndpointsFromSwagger(options);
        if (swaggerCatalog.Count > 0)
        {
            return swaggerCatalog;
        }

        return options.Resources.ToDictionary(
            entry => entry.Key,
            entry => MapConfiguredEndpoint(entry.Key, entry.Value),
            StringComparer.OrdinalIgnoreCase);
    }

    // Le o arquivo que contem o swagger do portal para evitar manter um catalogo manual.
    private static IReadOnlyDictionary<string, Endpoint> TryLoadEndpointsFromSwagger(TceApiOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.SwaggerUiInitPath) || !File.Exists(options.SwaggerUiInitPath))
        {
            return new Dictionary<string, Endpoint>(StringComparer.OrdinalIgnoreCase);
        }

        try
        {
            var content = File.ReadAllText(options.SwaggerUiInitPath);
            var swaggerJson = ExtractSwaggerJson(content);
            using var document = JsonDocument.Parse(swaggerJson);

            if (!document.RootElement.TryGetProperty("paths", out var pathsElement))
            {
                return new Dictionary<string, Endpoint>(StringComparer.OrdinalIgnoreCase);
            }

            var endpoints = new Dictionary<string, Endpoint>(StringComparer.OrdinalIgnoreCase);

            foreach (var pathEntry in pathsElement.EnumerateObject())
            {
                if (!pathEntry.Value.TryGetProperty("get", out var getElement))
                {
                    continue;
                }

                var key = pathEntry.Name.Trim('/');
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                var fields = ReadFields(getElement).ToArray();

                endpoints[key] = new Endpoint
                {
                    Key = key,
                    Path = key,
                    Category = ReadCategory(getElement),
                    Description = getElement.TryGetProperty("summary", out var summaryElement)
                        ? summaryElement.GetString() ?? $"Consulta do endpoint {key}"
                        : $"Consulta do endpoint {key}",
                    RequiredFields = fields.Where(field => field.Required).ToArray(),
                    OptionalFields = fields.Where(field => !field.Required).ToArray()
                };
            }

            return endpoints;
        }
        catch
        {
            return new Dictionary<string, Endpoint>(StringComparer.OrdinalIgnoreCase);
        }
    }

    // Extrai o documento swagger embutido dentro do script do portal.
    private static string ExtractSwaggerJson(string content)
    {
        const string marker = "\"swaggerDoc\":";
        var markerIndex = content.IndexOf(marker, StringComparison.Ordinal);
        if (markerIndex < 0)
        {
            throw new InvalidOperationException("Nao foi possivel localizar o swaggerDoc no arquivo do portal.");
        }

        var startIndex = content.IndexOf('{', markerIndex + marker.Length);
        if (startIndex < 0)
        {
            throw new InvalidOperationException("Nao foi possivel localizar o inicio do JSON do swagger.");
        }

        var depth = 0;
        var insideString = false;
        var escaped = false;

        for (var index = startIndex; index < content.Length; index++)
        {
            var current = content[index];

            if (insideString)
            {
                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (current == '\\')
                {
                    escaped = true;
                    continue;
                }

                if (current == '"')
                {
                    insideString = false;
                }

                continue;
            }

            if (current == '"')
            {
                insideString = true;
                continue;
            }

            if (current == '{')
            {
                depth++;
            }
            else if (current == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return content[startIndex..(index + 1)];
                }
            }
        }

        throw new InvalidOperationException("Nao foi possivel fechar o JSON do swagger.");
    }

    // Mapeia a definicao do swagger para um modelo de dominio amigavel ao front-end.
    private static IEnumerable<EndpointField> ReadFields(JsonElement getElement)
    {
        if (!getElement.TryGetProperty("parameters", out var parametersElement))
        {
            yield break;
        }

        foreach (var parameter in parametersElement.EnumerateArray())
        {
            if (!parameter.TryGetProperty("in", out var locationElement) ||
                !string.Equals(locationElement.GetString(), "query", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var name = parameter.GetProperty("name").GetString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            var schemaType = parameter.TryGetProperty("schema", out var schemaElement) &&
                             schemaElement.TryGetProperty("type", out var typeElement)
                ? typeElement.GetString() ?? "string"
                : "string";

            yield return new EndpointField
            {
                Name = name,
                Label = BuildFieldLabel(name),
                Description = parameter.TryGetProperty("description", out var descriptionElement)
                    ? descriptionElement.GetString() ?? string.Empty
                    : string.Empty,
                Type = InferFieldType(name, schemaType),
                Required = parameter.TryGetProperty("required", out var requiredElement) && requiredElement.GetBoolean()
            };
        }
    }

    // Define a categoria primaria a partir da primeira tag do swagger.
    private static string ReadCategory(JsonElement getElement)
    {
        if (!getElement.TryGetProperty("tags", out var tagsElement) || tagsElement.GetArrayLength() == 0)
        {
            return "Outros";
        }

        var rawTag = tagsElement[0].GetString() ?? "Outros";
        return rawTag
            .Replace("Documentacao referente a ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" - SIM", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();
    }

    // Construcao do fallback quando a leitura automatica do swagger nao estiver disponivel.
    private static Endpoint MapConfiguredEndpoint(string key, TceEndpointOption option)
    {
        return new Endpoint
        {
            Key = key,
            Path = option.Path,
            Category = option.Category,
            Description = option.Description,
            RequiredFields = option.RequiredQueryParameters
                .Select(name => new EndpointField
                {
                    Name = name,
                    Label = BuildFieldLabel(name),
                    Type = InferFieldType(name, "string"),
                    Required = true
                })
                .ToArray(),
            OptionalFields = option.OptionalQueryParameters
                .Select(name => new EndpointField
                {
                    Name = name,
                    Label = BuildFieldLabel(name),
                    Type = InferFieldType(name, "string"),
                    Required = false
                })
                .ToArray()
        };
    }

    // Monta os parametros efetivos da consulta incluindo municipio e pagina.
    private static IReadOnlyDictionary<string, string> BuildQueryParameters(Endpoint endpoint, QueryRequest request)
    {
        var queryParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var parameter in request.Parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameter.Value))
            {
                queryParameters[parameter.Key] = parameter.Value.Trim();
            }
        }

        // Injeta o municipio automaticamente quando o endpoint exige esse contexto.
        if (endpoint.RequiredFields.Any(field => string.Equals(field.Name, "codigo_municipio", StringComparison.OrdinalIgnoreCase)) ||
            endpoint.OptionalFields.Any(field => string.Equals(field.Name, "codigo_municipio", StringComparison.OrdinalIgnoreCase)))
        {
            queryParameters["codigo_municipio"] = request.MunicipalityCode.Trim();
        }

        // Quando o endpoint aceita paginacao por deslocamento, a aplicacao controla isso.
        if (UsesSourcePagination(endpoint))
        {
            queryParameters["quantidade"] = request.NormalizedPageSize.ToString();
            queryParameters["deslocamento"] = ((request.NormalizedPage - 1) * request.NormalizedPageSize).ToString();
        }

        ValidateRequiredParameters(endpoint, queryParameters);
        return queryParameters;
    }

    // Garante que os campos obrigatorios do endpoint foram preenchidos.
    private static void ValidateRequiredParameters(Endpoint endpoint, IReadOnlyDictionary<string, string> queryParameters)
    {
        var missingParameters = endpoint.RequiredFields
            .Where(field => !HiddenRequiredParameters.Contains(field.Name))
            .Where(field => !queryParameters.TryGetValue(field.Name, out var value) || string.IsNullOrWhiteSpace(value))
            .Select(field => field.Label)
            .ToArray();

        if (missingParameters.Length > 0)
        {
            throw new ArgumentException($"Preencha os campos obrigatorios: {string.Join(", ", missingParameters)}.");
        }

        if (endpoint.RequiredFields.Any(field => string.Equals(field.Name, "codigo_municipio", StringComparison.OrdinalIgnoreCase)) &&
            (!queryParameters.TryGetValue("codigo_municipio", out var municipalityCode) || string.IsNullOrWhiteSpace(municipalityCode)))
        {
            throw new ArgumentException("Selecione um municipio valido para realizar a consulta.");
        }
    }

    // Faz paginacao local para endpoints que devolvem tudo de uma vez.
    private static IReadOnlyList<IReadOnlyDictionary<string, string>> PaginateItems(
        Endpoint endpoint,
        QueryRequest request,
        IReadOnlyList<IReadOnlyDictionary<string, string>> items,
        out int totalItems,
        out int totalPages,
        out bool hasMorePages)
    {
        if (UsesSourcePagination(endpoint))
        {
            totalItems = items.Count == request.NormalizedPageSize
                ? ((request.NormalizedPage - 1) * request.NormalizedPageSize) + items.Count + 1
                : ((request.NormalizedPage - 1) * request.NormalizedPageSize) + items.Count;

            totalPages = totalItems == 0
                ? 0
                : (int)Math.Ceiling(totalItems / (double)request.NormalizedPageSize);

            hasMorePages = items.Count == request.NormalizedPageSize;
            return items;
        }

        totalItems = items.Count;
        totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)request.NormalizedPageSize);

        hasMorePages = request.NormalizedPage < totalPages;

        return items
            .Skip((request.NormalizedPage - 1) * request.NormalizedPageSize)
            .Take(request.NormalizedPageSize)
            .ToArray();
    }

    // Descobre todas as colunas da grade respeitando a ordem em que aparecem.
    private static IReadOnlyList<string> InferColumns(IReadOnlyList<IReadOnlyDictionary<string, string>> items)
    {
        var columns = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            foreach (var key in item.Keys)
            {
                if (seen.Add(key))
                {
                    columns.Add(key);
                }
            }
        }

        return columns;
    }

    // Identifica endpoints que usam quantidade e deslocamento na origem.
    private static bool UsesSourcePagination(Endpoint endpoint)
    {
        return endpoint.RequiredFields.Any(field => string.Equals(field.Name, "quantidade", StringComparison.OrdinalIgnoreCase)) &&
               endpoint.RequiredFields.Any(field => string.Equals(field.Name, "deslocamento", StringComparison.OrdinalIgnoreCase));
    }

    // Recupera um endpoint existente ou falha com mensagem clara.
    private Endpoint GetEndpoint(string endpointKey)
    {
        if (_endpoints.TryGetValue(endpointKey, out var endpoint))
        {
            return endpoint;
        }

        throw new ArgumentException($"O endpoint '{endpointKey}' nao esta configurado.");
    }

    // Gera um rotulo mais amigavel a partir do nome tecnico do campo.
    private static string BuildFieldLabel(string name)
    {
        var words = name.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return string.Join(" ", words.Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
    }

    // Escolhe o tipo de input a partir do schema e do nome do campo.
    private static string InferFieldType(string name, string schemaType)
    {
        if (name.Contains("data", StringComparison.OrdinalIgnoreCase))
        {
            return "date";
        }

        if (schemaType.Equals("integer", StringComparison.OrdinalIgnoreCase) ||
            schemaType.Equals("number", StringComparison.OrdinalIgnoreCase))
        {
            return "number";
        }

        return "text";
    }

    // Le a primeira chave disponivel de um item do retorno.
    private static string ReadFirst(IReadOnlyDictionary<string, string> item, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (item.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return string.Empty;
    }
}
