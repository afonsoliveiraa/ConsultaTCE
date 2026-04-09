using Application.Interfaces;
using Domain.Models;
using Infrastructure.Services;

namespace ConsultaTCE.Services;

// Adaptador fino que conecta a interface da camada de aplicacao com o servico
// concreto de infraestrutura, sem criar dependencia reversa entre projetos.
public class TceServiceAdapter : ITceService
{
    private readonly TceService _tceService;

    public TceServiceAdapter(TceService tceService)
    {
        _tceService = tceService;
    }

    public Task<IReadOnlyList<Domain.Entities.Endpoint>> GetEndpointsAsync(CancellationToken cancellationToken)
        => _tceService.GetEndpointsAsync(cancellationToken);

    public Task<IReadOnlyList<MunicipalityOption>> GetMunicipalitiesAsync(CancellationToken cancellationToken)
        => _tceService.GetMunicipalitiesAsync(cancellationToken);

    public Task<QueryResult> QueryAsync(QueryRequest request, CancellationToken cancellationToken)
        => _tceService.QueryAsync(request, cancellationToken);
}
