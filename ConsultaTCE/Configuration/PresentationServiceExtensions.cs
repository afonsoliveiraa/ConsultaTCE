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
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "ConsultaTCE API",
                Version = "v1"
            });
        });
        services.AddFrontendCors(configuration);

        return services;
    }

    public static WebApplication UsePresentationPipeline(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "swagger";
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ConsultaTCE API v1");
        });

        // A API aceita chamadas apenas das origens configuradas para o frontend.
        app.UseCors(CorsConfigurationExtensions.FrontendDevPolicy);
        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        // Os arquivos estaticos gerados pelo frontend sao servidos pelo host .NET.
        app.UseStaticFiles();

        return app;
    }
}
