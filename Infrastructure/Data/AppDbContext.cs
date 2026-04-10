using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Registra a Entidade 
    public DbSet<Contrato> Contratos { get; set; }
    public DbSet<Licitacao> Licitacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}