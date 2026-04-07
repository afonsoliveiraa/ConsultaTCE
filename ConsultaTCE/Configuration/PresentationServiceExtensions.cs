namespace ConsultaTCE.Configuration;

public static class PresentationServiceExtensions
{
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Mantem o bootstrap da camada web concentrado fora do Program.cs.
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddFrontendCors(configuration);

        return services;
    }

    public static WebApplication UsePresentationPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // Swagger fica disponivel apenas no fluxo de desenvolvimento.
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // A API aceita chamadas apenas das origens configuradas para o frontend.
        app.UseCors(CorsConfigurationExtensions.FrontendDevPolicy);
        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        // Os arquivos estaticos gerados pelo frontend sao servidos pelo host .NET.
        app.UseStaticFiles();

        return app;
    }
}
