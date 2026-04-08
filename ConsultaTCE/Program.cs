using Application.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.FileProcessors;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// 1. Registro dos Serviços de Infraestrutura e Banco
// --------------------------------------------------

// Busca a string de conexão e garante que não é nula
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configura o DbContext para PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Injeção de Dependência dos seus Processadores e Leitores
// Camada de Infra (Implementações)
builder.Services.AddScoped<ILeitorCO, LeitorCO>();
builder.Services.AddScoped<IContratoRepository, ContratoRepository>();

// Camada de Application (Orquestrador)
builder.Services.AddScoped<ContratoService>();

// --------------------------------------------------
// 2. Configuração da API e Swagger
// --------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "ConsultaTCE API",
        Version = "v1"
    });
});

// --------------------------------------------------
// 3. Configuração do CORS (Dinâmico)
// --------------------------------------------------

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

// --------------------------------------------------
// 4. Pipeline de Execução (Middlewares)
// --------------------------------------------------

// Documentação sempre disponível no ambiente de dev
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ConsultaTCE API v1");
});

app.UseHttpsRedirection();

// IMPORTANTE: UseCors deve vir antes de MapControllers
app.UseCors("FrontendDev");

app.UseAuthorization();

app.MapControllers();

// --------------------------------------------------
// 5. Redirecionamentos de Inicialização
// --------------------------------------------------

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/swagger", () => Results.Redirect("/swagger/index.html"));

app.Run();