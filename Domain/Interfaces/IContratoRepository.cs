using Domain.Entities;

namespace Domain.Interfaces;

public interface IContratoRepository
{
    Task AdicionarVariosAsync(IEnumerable<Contrato> contratos);
    Task<(IReadOnlyList<Contrato> Contratos, int TotalItems)> BuscarPorContratoAsync(
        string? numeroContrato,
        int page,
        int pageSize);
}
