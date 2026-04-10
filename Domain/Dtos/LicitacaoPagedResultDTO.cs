namespace Domain.Dtos;

public record LicitacaoPagedResultDTO(
    IReadOnlyList<LicitacaoDTO> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages
);