using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.FileProcessors;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // Necessário para OpenApiSecurityScheme

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// 1. Registro dos Serviços de Infraestrutura e Banco
// --------------------------------------------------

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ILeitorCO, LeitorCO>();
builder.Services.AddScoped<IContratoRepository, ContratoRepository>();
builder.Services.AddScoped<ContratoService>();

// --------------------------------------------------
// 2. Configuração da API e Swagger (Com campo de Chave)
// --------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "ConsultaTCE API", Version = "v1" });

    // Configura o botão "Authorize" no Swagger para enviar o X-App-Secret
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Insira a chave secreta definida no appsettings ou GitLab (X-App-Secret)",
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
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// --------------------------------------------------
// 3. Configuração do CORS
// --------------------------------------------------

var allowedOrigins = builder.Configuration
    .GetSection("Frontend:AllowedOrigins")
    .Get<string[]>() ?? ["https://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// --------------------------------------------------
// 4. Pipeline de Execução (Middlewares)
// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "swagger";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ConsultaTCE API v1");
    });
}

app.UseHttpsRedirection();

// Importante: O CORS deve vir antes de qualquer bloqueio
app.UseCors("FrontendDev");

// --- MIDDLEWARE DE VALIDAÇÃO DA CHAVE ---
app.Use(async (context, next) =>
{
    // Permite livre acesso ao Swagger e à raiz
    var path = context.Request.Path.Value?.ToLower();
    if (path == "/" || path!.StartsWith("/swagger"))
    {
        await next();
        return;
    }

    // Busca a chave configurada
    var secretKey = builder.Configuration["Security:FrontendKey"];

    // Se o header não existir ou for diferente da chave configurada, bloqueia
    if (!context.Request.Headers.TryGetValue("X-App-Secret", out var extractedKey) || 
        extractedKey != secretKey)
    {
        context.Response.StatusCode = 403;
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Acesso negado: Somente o front-end autorizado pode realizar requisições.");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapControllers();

// --------------------------------------------------
// 5. Redirecionamentos
// --------------------------------------------------

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();