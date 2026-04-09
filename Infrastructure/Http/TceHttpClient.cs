using System.Text.Json;
using System.Text.Json.Nodes;
using Infrastructure.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Http;

// Cliente HTTP especializado em consultar a API publica do TCE-CE e devolver
// um formato normalizado para a camada de servico.
public class TceHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<TceHttpClient> _logger;
    private readonly TceApiOptions _options;

    public TceHttpClient(
        HttpClient httpClient,
        IMemoryCache memoryCache,
        IOptions<TceApiOptions> options,
        ILogger<TceHttpClient> logger)
    {
        _httpClient = httpClient;
        _memoryCache = memoryCache;
        _logger = logger;
        _options = options.Value;
    }

    // Executa a consulta remota respeitando cache e serializando o retorno
    // para uma colecao uniforme de objetos JSON.
    public async Task<TceHttpResponse> GetAsync(
        string path,
        IReadOnlyDictionary<string, string> queryParameters,
        CancellationToken cancellationToken)
    {
        var sourceUrl = BuildSourceUrl(path, queryParameters);
        var cacheKey = $"tce-http::{sourceUrl}";

        if (_memoryCache.TryGetValue(cacheKey, out TceHttpResponse? cachedResponse) && cachedResponse is not null)
        {
            _logger.LogInformation("Cache hit na consulta do TCE-CE para {Url}", sourceUrl);
            return new TceHttpResponse
            {
                SourceUrl = cachedResponse.SourceUrl,
                Items = cachedResponse.Items,
                Metadata = CloneMetadata(cachedResponse.Metadata, true)
            };
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, sourceUrl);

        // O header e opcional para manter compatibilidade com ambientes diferentes.
        if (!string.IsNullOrWhiteSpace(_options.ApiKeyHeaderName) &&
            !string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            request.Headers.TryAddWithoutValidation(_options.ApiKeyHeaderName, _options.ApiKey);
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Falha ao consultar '{path}' no TCE-CE. Status: {(int)response.StatusCode}. Resposta: {body}");
        }

        var normalized = NormalizeResponse(body);
        var metadata = CloneMetadata(normalized.Metadata, false);
        metadata["statusCode"] = ((int)response.StatusCode).ToString();

        var materializedResponse = new TceHttpResponse
        {
            SourceUrl = sourceUrl,
            Items = normalized.Items,
            Metadata = metadata
        };

        _memoryCache.Set(cacheKey, materializedResponse, TimeSpan.FromSeconds(_options.CacheSeconds));
        return materializedResponse;
    }

    // Monta a URL final preservando o formato que a API publica espera.
    private string BuildSourceUrl(string path, IReadOnlyDictionary<string, string> queryParameters)
    {
        var baseUrl = _options.BaseUrl.EndsWith('/') ? _options.BaseUrl : $"{_options.BaseUrl}/";
        var cleanPath = path.TrimStart('/');

        if (queryParameters.Count == 0)
        {
            return new Uri(new Uri(baseUrl), cleanPath).ToString();
        }

        var queryString = string.Join(
            "&",
            queryParameters
                .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value))
                .Select(parameter => $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value)}"));

        return $"{new Uri(new Uri(baseUrl), cleanPath)}?{queryString}";
    }

    // Normaliza diferentes formatos de payload para uma lista simples de objetos.
    private static TceHttpResponse NormalizeResponse(string rawJson)
    {
        var root = JsonNode.Parse(rawJson)
            ?? throw new InvalidOperationException("A API do TCE-CE retornou um JSON vazio.");

        return root switch
        {
            JsonArray array => new TceHttpResponse
            {
                Items = array.Select(ConvertToDictionary).ToArray(),
                Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            },
            JsonObject obj => NormalizeObject(obj),
            _ => new TceHttpResponse
            {
                Items = [],
                Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["rawValue"] = root.ToJsonString()
                }
            }
        };
    }

    // Trata objetos que tragam itens em propriedades como data, items ou records.
    private static TceHttpResponse NormalizeObject(JsonObject jsonObject)
    {
        foreach (var propertyName in new[] { "data", "items", "results", "records" })
        {
            var propertyValue = jsonObject[propertyName];

            if (propertyValue is JsonValue jsonValue &&
                jsonValue.TryGetValue<string>(out var rawNestedJson) &&
                TryParseNestedJson(rawNestedJson) is JsonArray parsedArray)
            {
                return new TceHttpResponse
                {
                    Items = parsedArray.Select(ConvertToDictionary).ToArray(),
                    Metadata = jsonObject
                        .Where(entry => !string.Equals(entry.Key, propertyName, StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(
                            entry => entry.Key,
                            entry => entry.Value?.ToJsonString() ?? string.Empty,
                            StringComparer.OrdinalIgnoreCase)
                };
            }

            if (propertyValue is JsonObject wrappedObject &&
                TryExtractWrappedItems(wrappedObject, out var wrappedItems, out var wrappedMetadata))
            {
                return new TceHttpResponse
                {
                    Items = wrappedItems,
                    Metadata = jsonObject
                        .Where(entry => !string.Equals(entry.Key, propertyName, StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(
                            entry => entry.Key,
                            entry => entry.Value?.ToJsonString() ?? string.Empty,
                            StringComparer.OrdinalIgnoreCase)
                        .Concat(wrappedMetadata)
                        .ToDictionary(
                            entry => entry.Key,
                            entry => entry.Value,
                            StringComparer.OrdinalIgnoreCase)
                };
            }

            if (propertyValue is not JsonArray array)
            {
                continue;
            }

            return new TceHttpResponse
            {
                Items = array.Select(ConvertToDictionary).ToArray(),
                Metadata = jsonObject
                    .Where(entry => !string.Equals(entry.Key, propertyName, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value?.ToJsonString() ?? string.Empty,
                        StringComparer.OrdinalIgnoreCase)
            };
        }

        return new TceHttpResponse
        {
            Items = [ConvertToDictionary(jsonObject)],
            Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        };
    }

    // Converte cada item JSON em dicionario de string para deixar o contrato
    // do frontend totalmente generico.
    private static IReadOnlyDictionary<string, string> ConvertToDictionary(JsonNode? node)
    {
        if (node is JsonValue jsonValue &&
            jsonValue.TryGetValue<string>(out var rawNestedJson) &&
            TryParseNestedJson(rawNestedJson) is JsonObject parsedObject)
        {
            return ConvertToDictionary(parsedObject);
        }

        if (node is not JsonObject jsonObject)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["value"] = node?.ToJsonString() ?? string.Empty
            };
        }

        return jsonObject.ToDictionary(
            entry => entry.Key,
            entry => ConvertJsonValue(entry.Value),
            StringComparer.OrdinalIgnoreCase);
    }

    // Converte qualquer valor JSON em texto legivel para exibicao na grade.
    private static string ConvertJsonValue(JsonNode? value)
    {
        return value switch
        {
            null => string.Empty,
            JsonValue jsonValue when jsonValue.TryGetValue<string>(out var stringValue) &&
                                     TryParseNestedJson(stringValue) is JsonNode parsedNode
                => parsedNode is JsonArray array
                    ? string.Join(" | ", array.Select(ConvertJsonValue))
                    : parsedNode is JsonObject parsedObject
                        ? string.Join(" | ", parsedObject.Select(entry => $"{entry.Key}: {ConvertJsonValue(entry.Value)}"))
                        : stringValue,
            JsonValue jsonValue when jsonValue.TryGetValue<string>(out var stringValue) => stringValue,
            JsonValue jsonValue when jsonValue.TryGetValue<bool>(out var boolValue) => boolValue ? "Sim" : "Nao",
            JsonValue jsonValue when jsonValue.TryGetValue<DateTime>(out var dateValue) => dateValue.ToString("dd/MM/yyyy HH:mm:ss"),
            JsonValue jsonValue => jsonValue.ToJsonString().Trim('"'),
            JsonArray or JsonObject => value.ToJsonString(new JsonSerializerOptions { WriteIndented = false }),
            _ => value?.ToJsonString() ?? string.Empty
        };
    }

    // Alguns endpoints trazem o payload real em data.data, items.items ou estruturas equivalentes.
    private static bool TryExtractWrappedItems(
        JsonObject wrappedObject,
        out IReadOnlyList<IReadOnlyDictionary<string, string>> items,
        out Dictionary<string, string> metadata)
    {
        foreach (var nestedPropertyName in new[] { "data", "items", "results", "records" })
        {
            var nestedValue = wrappedObject[nestedPropertyName];

            if (nestedValue is JsonArray nestedArray)
            {
                items = nestedArray.Select(ConvertToDictionary).ToArray();
                metadata = wrappedObject
                    .Where(entry => !string.Equals(entry.Key, nestedPropertyName, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value?.ToJsonString() ?? string.Empty,
                        StringComparer.OrdinalIgnoreCase);
                return true;
            }

            if (nestedValue is JsonValue nestedJsonValue &&
                nestedJsonValue.TryGetValue<string>(out var rawNestedJson) &&
                TryParseNestedJson(rawNestedJson) is JsonArray parsedNestedArray)
            {
                items = parsedNestedArray.Select(ConvertToDictionary).ToArray();
                metadata = wrappedObject
                    .Where(entry => !string.Equals(entry.Key, nestedPropertyName, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value?.ToJsonString() ?? string.Empty,
                        StringComparer.OrdinalIgnoreCase);
                return true;
            }
        }

        items = [];
        metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        return false;
    }

    // Alguns endpoints devolvem objetos ou listas JSON serializados como string.
    private static JsonNode? TryParseNestedJson(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return null;
        }

        var trimmedValue = rawValue.Trim();
        if (!trimmedValue.StartsWith('{') && !trimmedValue.StartsWith('['))
        {
            return null;
        }

        try
        {
            return JsonNode.Parse(trimmedValue);
        }
        catch
        {
            return null;
        }
    }

    // Clona os metadados antes de devolver a resposta para evitar mutacao acidental.
    private static Dictionary<string, string> CloneMetadata(
        IReadOnlyDictionary<string, string> metadata,
        bool cacheHit)
    {
        var clone = new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase)
        {
            ["cacheHit"] = cacheHit ? "true" : "false"
        };

        return clone;
    }
}

// Estrutura de transporte interna entre o cliente HTTP e o servico de infraestrutura.
public class TceHttpResponse
{
    public string SourceUrl { get; init; } = string.Empty;

    public IReadOnlyList<IReadOnlyDictionary<string, string>> Items { get; init; } = [];

    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
