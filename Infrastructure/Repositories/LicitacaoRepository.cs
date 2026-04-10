using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LicitacaoRepository : ILicitacaoRepository
{
    private readonly AppDbContext _context;

    public LicitacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarVariosAsync(IEnumerable<Licitacao> licitacoes)
    {
        // Prepara a inserção em lote para o PostgreSQL
        await _context.Licitacoes.AddRangeAsync(licitacoes);
        
        // Commita as licitações no banco
        await _context.SaveChangesAsync();
    }

    public async Task<(IReadOnlyList<Licitacao> Licitacoes, int TotalItems)> BuscarPorProcessoAsync(
        string? numeroProcesso,
        int page,
        int pageSize)
    {
        // AsNoTracking para performance em consultas de leitura
        var query = _context.Licitacoes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(numeroProcesso))
        {
            // Filtro pelo número do processo administrativo (Campo 4 do layout)
            query = query.Where(l => l.NumeroProcesso == numeroProcesso);
        }

        var totalItems = await query.CountAsync();

        var licitacoes = await query
            .OrderByDescending(l => l.DataAutuacao) // Geralmente licitações são ordenadas pela mais recente
            .ThenBy(l => l.NumeroProcesso)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (licitacoes, totalItems);
    }
}