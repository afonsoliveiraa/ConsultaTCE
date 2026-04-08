using Domain.Entities;

namespace Domain.Interfaces;

public interface IContratoRepository
{
    Task AdicionarVariosAsync(IEnumerable<Contrato> contratos);
    Task<IEnumerable<Contrato>> BuscarPorContratoAsync(string numeroContrato);
}