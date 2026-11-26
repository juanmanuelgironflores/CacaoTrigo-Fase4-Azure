using CacaoTrigo.Models;

namespace CacaoTrigo.Repositories;

public interface IDetallePedidoRepository
{
    Task<List<DetallePedido>> ObtenerTodosAsync();
    Task<List<DetallePedido>> ObtenerPorPedidosAsync(IReadOnlyCollection<int> pedidoIds);
    Task<DetallePedido?> ObtenerPorIdAsync(int id);
    Task<Pedido?> ObtenerPedidoPorIdAsync(int id);
    Task<Producto?> ObtenerProductoPorIdAsync(int id);
    Task AgregarAsync(DetallePedido detallePedido);
    Task GuardarCambiosAsync();
    Task EliminarAsync(DetallePedido detallePedido);
    Task RecalcularTotalPedidoAsync(int pedidoId);
}
