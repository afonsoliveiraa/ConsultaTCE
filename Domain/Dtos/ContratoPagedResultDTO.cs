namespace Domain.Dtos;

public record ContratoPagedResultDTO(
    IReadOnlyList<ContratoDTO> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages
);
