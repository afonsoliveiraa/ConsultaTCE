using ConsultaTCE.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Registra apenas os servicos da camada de apresentacao.
builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();

// Aplica o pipeline HTTP e serve a base do frontend.
app.UsePresentationPipeline();
app.MapControllers();
app.MapGet("/swagger", () => Results.Redirect("/swagger/index.html"));
app.MapFallbackToFile("index.html");

app.Run();
