var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// Registro dos servicos da aplicacao
// --------------------------------------------------

// Habilita suporte a Controllers.
builder.Services.AddControllers();

// Habilita exploracao de endpoints para Swagger/OpenAPI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "ConsultaTCE API",
        Version = "v1"
    });
});

// Configura o CORS para o frontend em desenvolvimento e para a propria origem do host .NET.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:5173",
                "https://localhost:7113")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// --------------------------------------------------

// Publica a documentacao OpenAPI e a interface do Swagger.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ConsultaTCE API v1");
});

// Redireciona requisicoes HTTP para HTTPS.
app.UseHttpsRedirection();

// Aplica a politica de CORS antes do mapeamento de rotas.
app.UseCors("FrontendDev");

// --------------------------------------------------
// Mapeamento de rotas
// --------------------------------------------------

// Publica os controllers da API.
app.MapControllers();

// Quando a origem do backend for acessada diretamente, abre o Swagger.
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/swagger", () => Results.Redirect("/swagger/index.html"));

app.Run();
