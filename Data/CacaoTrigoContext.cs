using CacaoTrigo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Data;

public class CacaoTrigoContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Insumo> Insumos => Set<Insumo>();
    public DbSet<Mesa> Mesas => Set<Mesa>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
    public DbSet<Promocion> Promociones => Set<Promocion>();
    public DbSet<ProductoInsumo> ProductosInsumos => Set<ProductoInsumo>();

    public CacaoTrigoContext(
        DbContextOptions<CacaoTrigoContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producto>()
            .Property(p => p.Precio)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Pedido>()
            .Property(p => p.Total)
            .HasPrecision(10, 2);

        modelBuilder.Entity<DetallePedido>()
            .Property(d => d.PrecioUnitario)
            .HasPrecision(10, 2);

        modelBuilder.Entity<DetallePedido>()
            .Property(d => d.Subtotal)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Promocion>()
            .Property(p => p.ValorDescuento)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Insumo>()
            .Property(i => i.CantidadDisponible)
            .HasPrecision(12, 3);

        modelBuilder.Entity<ProductoInsumo>()
            .Property(pi => pi.CantidadNecesaria)
            .HasPrecision(12, 3);

        // Cliente dueño del pedido / reserva (opcional).
        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Usuario)
            .WithMany()
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Usuario)
            .WithMany()
            .HasForeignKey(r => r.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        // La mesa es opcional: la asigna el administrador al confirmar.
        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Mesa)
            .WithMany()
            .HasForeignKey(r => r.MesaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}