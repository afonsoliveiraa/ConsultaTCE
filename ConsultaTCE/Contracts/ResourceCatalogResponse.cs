namespace ConsultaTCE.Contracts;

internal sealed record ResourceCatalogResponse(
    string BaseUrl,
    IReadOnlyList<ResourceDescriptor> Resources);
