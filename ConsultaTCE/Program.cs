var builder = WebApplication.CreateBuilder(args);

// 1. Adiciona os serviços do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();      // Gera o JSON do Swagger
    app.UseSwaggerUI();    // Gera a interface gráfica (A página que você acessa)
}

app.UseHttpsRedirection();

// Define a rota base da aplicação para o swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();