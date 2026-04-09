using Domain.Entities;
using Domain.Models;

namespace Application.Interfaces;

// Contrato que separa a camada de aplicacao da implementacao concreta
// de acesso ao catalogo e aos dados do TCE-CE.
public interface ITceService
{
    // Retorna os endpoints disponiveis para popular o seletor da tela.
    Task<IReadOnlyList<Endpoint>> GetEndpointsAsync(CancellationToken cancellationToken);

    // Retorna os municipios para o usuario escolher o contexto da busca.
    Task<IReadOnlyList<MunicipalityOption>> GetMunicipalitiesAsync(CancellationToken cancellationToken);

    // Executa a consulta dinamica em um endpoint do TCE-CE.
    Task<QueryResult> QueryAsync(QueryRequest request, CancellationToken cancellationToken);
}
