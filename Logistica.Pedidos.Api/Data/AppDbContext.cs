using Microsoft.EntityFrameworkCore;
using Logistica.Pedidos.Api.Models;

namespace Logistica.Pedidos.Api.Data;

// DbContext = "ponte" entre o C# e o banco SQL
public class AppDbContext : DbContext
{
    // DbContext = "ponte" entre o C# e o banco SQL
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Representa a tabela Pedidos
    public DbSet<Pedido> Pedidos => Set<Pedido>();

    //
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
     modelBuilder.Entity<Pedido>()
            .Property(p => p.ValorTotal)
           .HasColumnType("decimal(18,2)");
    }

}




