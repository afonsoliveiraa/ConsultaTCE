using ConsultaTCE.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Registra apenas os servicos da camada de apresentacao.
builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();

// Aplica o pipeline HTTP e publica os controllers da API.
app.UsePresentationPipeline();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
