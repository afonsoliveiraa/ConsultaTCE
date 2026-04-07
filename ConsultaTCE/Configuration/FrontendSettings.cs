namespace ConsultaTCE.Configuration;

public sealed class FrontendSettings
{
    public const string SectionName = "Frontend";

    // URLs e origens usadas para integrar o frontend local com a API.
    public string[] AllowedOrigins { get; init; } = new[]
    {
        "https://localhost:3000"
    };

    public string DevelopmentUrl { get; init; } = "https://localhost:3000";
    public string BackendHttpsUrl { get; init; } = "https://localhost:7113";

    public string[] GetAllowedOrigins()
    {
        return AllowedOrigins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
