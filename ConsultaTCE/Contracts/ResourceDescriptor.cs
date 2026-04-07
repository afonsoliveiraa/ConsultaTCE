namespace ConsultaTCE.Contracts;

internal sealed record ResourceDescriptor(
    string Key,
    string Path,
    string? Category,
    string? Description,
    IReadOnlyList<string> RequiredQueryParameters,
    IReadOnlyList<string> OptionalQueryParameters,
    IReadOnlyList<QueryParameterDescriptor> QueryParameters,
    bool RequiresAuthentication = false);
