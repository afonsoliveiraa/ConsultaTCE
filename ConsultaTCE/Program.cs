using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "https://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

var resources = new[]
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendDev");
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    service = "ConsultaTCE",
    timestampUtc = DateTime.UtcNow,
}));

app.MapGet("/api/resources", (HttpContext httpContext) =>
{
    var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

    return Results.Ok(new ResourceCatalogResponse(baseUrl, resources));
});

app.MapGet("/api/resources/{resource}", (
    string resource,
    int? page,
    int? pageSize,
    string? codigo_municipio,
    string? termo,
    HttpContext httpContext) =>
{
    var normalizedPage = Math.Max(1, page ?? 1);
    var normalizedPageSize = Math.Clamp(pageSize ?? 25, 1, 250);
    var sourceUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}";

    if (resource.Equals("diagnostico_backend", StringComparison.OrdinalIgnoreCase))
    {
        var item = new Dictionary<string, object?>
        {
            ["servico"] = "ConsultaTCE API",
            ["ambiente"] = app.Environment.EnvironmentName,
            ["porta_http"] = "http://localhost:5130",
            ["porta_https"] = "https://localhost:7113",
            ["frontend_esperado"] = "http://localhost:3000",
            ["status"] = "Conectado",
            ["timestamp_utc"] = DateTime.UtcNow.ToString("O"),
        };

        return Results.Ok(BuildEnvelope(resource, sourceUrl, normalizedPage, normalizedPageSize, new[] { item }));
    }

    if (resource.Equals("consulta_exemplo", StringComparison.OrdinalIgnoreCase))
    {
        var allItems = Enumerable.Range(1, 60)
            .Select(index => new Dictionary<string, object?>
            {
                ["id"] = index,
                ["municipio"] = string.IsNullOrWhiteSpace(codigo_municipio) ? "NAO INFORMADO" : codigo_municipio,
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

        return Results.Ok(BuildEnvelope(resource, sourceUrl, normalizedPage, normalizedPageSize, filteredItems));
    }

    return Results.NotFound(new ProblemDetails
    {
        Title = "Recurso nao encontrado",
        Detail = $"O recurso '{resource}' nao esta disponivel nesta API.",
        Status = StatusCodes.Status404NotFound,
    });
});

app.MapFallbackToFile("index.html");

app.Run();

static PaginatedEnvelope BuildEnvelope(
    string resource,
    string sourceUrl,
    int page,
    int pageSize,
    IReadOnlyList<Dictionary<string, object?>> allItems)
{
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

internal sealed record QueryParameterDescriptor(
    string Name,
    bool Required,
    string? Description,
    string? Type);

internal sealed record ResourceDescriptor(
    string Key,
    string Path,
    string? Category,
    string? Description,
    IReadOnlyList<string> RequiredQueryParameters,
    IReadOnlyList<string> OptionalQueryParameters,
    IReadOnlyList<QueryParameterDescriptor> QueryParameters,
    bool RequiresAuthentication = false);

internal sealed record ResourceCatalogResponse(
    string BaseUrl,
    IReadOnlyList<ResourceDescriptor> Resources);

internal sealed record PaginatedEnvelope(
    string Resource,
    string SourceUrl,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Items,
    IReadOnlyDictionary<string, object?> Metadata,
    DateTime CachedAtUtc,
    DateTime ExpiresAtUtc);
