using Microsoft.Extensions.Options;

namespace ConsultaTCE.Configuration;

public static class CorsConfigurationExtensions
{
    public const string FrontendDevPolicy = "FrontendDev";

    public static IServiceCollection AddFrontendCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Le a configuracao do frontend uma unica vez para montar a politica de CORS.
        var frontendSettings = configuration
            .GetSection(FrontendSettings.SectionName)
            .Get<FrontendSettings>() ?? new FrontendSettings();
        var allowedOrigins = frontendSettings.GetAllowedOrigins();

        services.Configure<FrontendSettings>(configuration.GetSection(FrontendSettings.SectionName));
        services.AddSingleton<IValidateOptions<FrontendSettings>, FrontendSettingsValidation>();
        services.AddCors(options =>
        {
            options.AddPolicy(FrontendDevPolicy, policy =>
            {
                // Em desenvolvimento, apenas o frontend configurado pode consumir a API diretamente.
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}

internal sealed class FrontendSettingsValidation : IValidateOptions<FrontendSettings>
{
    public ValidateOptionsResult Validate(string? name, FrontendSettings options)
    {
        if (options.GetAllowedOrigins().Length == 0)
        {
            return ValidateOptionsResult.Fail("Frontend:AllowedOrigins precisa conter pelo menos uma origem.");
        }

        if (string.IsNullOrWhiteSpace(options.BackendHttpsUrl))
        {
            return ValidateOptionsResult.Fail("Frontend:BackendHttpsUrl precisa ser definido.");
        }

        // As configuracoes minimas foram informadas corretamente.
        return ValidateOptionsResult.Success;
    }
}
