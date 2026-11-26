using CacaoTrigo.Data;
using CacaoTrigo.Models;
using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Repositories;

public class DetallePedidoRepository : IDetallePedidoRepository
{
    private readonly CacaoTrigoContext _context;

    public DetallePedidoRepository(CacaoTrigoContext context)
    {
        _context = context;
    }

    public async Task<List<DetallePedido>> ObtenerTodosAsync()
    {
        return await _context.DetallesPedido
            .Include(d => d.Pedido)
            .Include(d => d.Producto)
            .OrderBy(d => d.PedidoId)
            .ToListAsync();
    }

    public async Task<List<DetallePedido>> ObtenerPorPedidosAsync(
        IReadOnlyCollection<int> pedidoIds)
    {
        if (pedidoIds.Count == 0)
        {
            return new List<DetallePedido>();
        }

        return await _context.DetallesPedido
            .Include(d => d.Producto)
            .Where(d => pedidoIds.Contains(d.PedidoId))
            .ToListAsync();
    }

    public async Task<DetallePedido?> ObtenerPorIdAsync(int id)
    {
        return await _context.DetallesPedido
            .Include(d => d.Pedido)
            .Include(d => d.Producto)
            .FirstOrDefaultAsync(d => d.DetallePedidoId == id);
    }

    public async Task<Pedido?> ObtenerPedidoPorIdAsync(int id)
    {
        return await _context.Pedidos
            .FirstOrDefaultAsync(p => p.PedidoId == id);
    }

    public async Task<Producto?> ObtenerProductoPorIdAsync(int id)
    {
        return await _context.Productos
            .FirstOrDefaultAsync(p => p.ProductoId == id);
    }

    public async Task AgregarAsync(DetallePedido detallePedido)
    {
        await _context.DetallesPedido.AddAsync(detallePedido);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task EliminarAsync(DetallePedido detallePedido)
    {
        _context.DetallesPedido.Remove(detallePedido);
        return Task.CompletedTask;
    }

    public async Task RecalcularTotalPedidoAsync(int pedidoId)
    {
        var pedido = await _context.Pedidos.FindAsync(pedidoId);

        if (pedido == null)
        {
            return;
        }

        pedido.Total = await _context.DetallesPedido
            .Where(d => d.PedidoId == pedidoId)
            .SumAsync(d => (decimal?)d.Subtotal) ?? 0m;

        await _context.SaveChangesAsync();
    }
}
