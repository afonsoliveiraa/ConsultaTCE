namespace ConsultaTCE.Contracts;

internal sealed record QueryParameterDescriptor(
    string Name,
    bool Required,
    string? Description,
    string? Type);
