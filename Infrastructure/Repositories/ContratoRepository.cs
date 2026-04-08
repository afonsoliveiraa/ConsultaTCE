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

    public async Task<IEnumerable<Contrato>> BuscarPorContratoAsync(string numeroContrato)
    {
        // O .Where retorna todos os registros que satisfazem a condição
        return await _context.Contratos
            .Where(n => n.NumeroContrato == numeroContrato)
            .ToListAsync();
    }
}