using Application.Interfaces;
using Application.Services;
using ConsultaTCE.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.FileProcessors;
using Infrastructure.Http;
using Infrastructure.Options;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Busca a string de conexão do banco e falha cedo quando a configuração não existir.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Registra o contexto do PostgreSQL usado pelo módulo de contratos.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Registra as implementações da infraestrutura local e da integração externa do TCE.
builder.Services.AddScoped<ILeitorCO, LeitorCO>();
builder.Services.AddScoped<IContratoRepository, ContratoRepository>();
builder.Services.AddScoped<TceService>();
builder.Services.AddScoped<ITceService, TceServiceAdapter>();
builder.Services.AddMemoryCache();
builder.Services.Configure<TceApiOptions>(
    builder.Configuration.GetSection(TceApiOptions.SectionName));

// Cliente HTTP usado para consultar a API pública do TCE.
builder.Services.AddHttpClient<TceHttpClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Registra os serviços de aplicação que orquestram os casos de uso.
builder.Services.AddScoped<ContratoService>();
builder.Services.AddScoped<TceAppService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ConsultaTCE API",
        Version = "v1"
    });

    // Mantém o Swagger apto a enviar o header interno usado pelo frontend.
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Insira a chave secreta definida no appsettings (X-App-Secret).",
        Name = "X-App-Secret",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });
});

// Libera o frontend configurado para consumir a API em desenvolvimento.
var allowedOrigins = builder.Configuration
    .GetSection("Frontend:AllowedOrigins")
    .Get<string[]>() ?? ["https://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "swagger";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ConsultaTCE API v1");
    });
}

// Em desenvolvimento a API também atende via HTTP para evitar falhas do proxy local.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("FrontendDev");
app.UseDefaultFiles();
app.UseStaticFiles();

// Protege as rotas internas da API para que apenas o frontend autorizado as consuma.
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLowerInvariant();

    if (string.IsNullOrWhiteSpace(path) ||
        !path.StartsWith("/api", StringComparison.Ordinal) ||
        path.Equals("/api-tce", StringComparison.Ordinal) ||
        path.StartsWith("/swagger", StringComparison.Ordinal))
    {
        await next();
        return;
    }

    var secretKey = builder.Configuration["Security:FrontendKey"];

    if (!context.Request.Headers.TryGetValue("X-App-Secret", out var extractedKey) ||
        extractedKey != secretKey)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Acesso negado: Somente o front-end autorizado pode realizar requisicoes.");
        return;
    }

    await next();
});

app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/swagger"));
    app.MapGet("/swagger", () => Results.Redirect("/swagger/index.html"));
}

app.Run();
