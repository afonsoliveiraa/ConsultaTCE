namespace ConsultaTCE.Contracts;

internal sealed record PaginatedEnvelope(
    string Resource,
    string SourceUrl,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Items,
    IReadOnlyDictionary<string, object?> Metadata,
    DateTime CachedAtUtc,
    DateTime ExpiresAtUtc);
