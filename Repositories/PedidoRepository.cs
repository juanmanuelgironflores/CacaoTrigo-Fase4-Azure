using CacaoTrigo.Data;
using CacaoTrigo.Models;
using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly CacaoTrigoContext _context;

    public PedidoRepository(CacaoTrigoContext context)
    {
        _context = context;
    }

    public async Task<List<Pedido>> ObtenerTodosAsync()
    {
        return await _context.Pedidos
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();
    }

    public async Task<List<Pedido>> ObtenerPorUsuarioAsync(string usuarioId)
    {
        return await _context.Pedidos
            .Where(p => p.UsuarioId == usuarioId)
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();
    }

    public async Task<Pedido?> ObtenerPorIdAsync(int id)
    {
        return await _context.Pedidos
            .FirstOrDefaultAsync(p => p.PedidoId == id);
    }

    public async Task AgregarAsync(Pedido pedido)
    {
        await _context.Pedidos.AddAsync(pedido);
    }

    public async Task AgregarConDetallesAsync(
        Pedido pedido,
        IReadOnlyCollection<DetallePedido> detalles)
    {
        await _context.Pedidos.AddAsync(pedido);
        foreach (var detalle in detalles)
        {
            detalle.Pedido = pedido;
        }

        await _context.DetallesPedido.AddRangeAsync(detalles);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task EliminarAsync(Pedido pedido)
    {
        _context.Pedidos.Remove(pedido);
        return Task.CompletedTask;
    }
}
