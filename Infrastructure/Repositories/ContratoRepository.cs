using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ContratoRepository : IContratoRepository
{
    private readonly AppDbContext _context;

    public ContratoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarVariosAsync(IEnumerable<Contrato> notas)
    {
        // O AddRangeAsync prepara as entidades para inserção em lote (bulk)
        await _context.Contratos.AddRangeAsync(notas);
        
        // O SaveChangesAsync executa o INSERT real no PostgreSQL
        await _context.SaveChangesAsync();
    }

    public async Task<(IReadOnlyList<Contrato> Contratos, int TotalItems)> BuscarPorContratoAsync(
        string? numeroContrato,
        int page,
        int pageSize)
    {
        var query = _context.Contratos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(numeroContrato))
        {
            query = query.Where(n => n.NumeroContrato == numeroContrato);
        }

        var totalItems = await query.CountAsync();

        var contratos = await query
            .OrderBy(n => n.NumeroContrato)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (contratos, totalItems);
    }
}
