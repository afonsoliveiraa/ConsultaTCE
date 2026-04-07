namespace ConsultaTCE.Configuration;

public sealed class FrontendSettings
{
    public const string SectionName = "Frontend";

    // URLs e origens usadas para integrar o frontend local com a API.
    public string[] AllowedOrigins { get; init; } = new[]
    {
        "http://localhost:3000",
        "https://localhost:3000"
    };

    public string DevelopmentUrl { get; init; } = "http://localhost:3000";
    public string BackendHttpUrl { get; init; } = "http://localhost:5130";
    public string BackendHttpsUrl { get; init; } = "https://localhost:7113";

    public string[] GetAllowedOrigins()
    {
        return AllowedOrigins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
