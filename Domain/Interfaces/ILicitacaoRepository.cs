using Domain.Entities;

namespace Domain.Interfaces;

public interface ILicitacaoRepository
{
    // Adiciona as licitações em lote (bulk insert)
    Task AdicionarVariosAsync(IEnumerable<Licitacao> licitacoes);

    // Busca paginada por número do processo administrativo
    Task<(IReadOnlyList<Licitacao> Licitacoes, int TotalItems)> BuscarPorProcessoAsync(
        string? numeroProcesso,
        int page,
        int pageSize);
}