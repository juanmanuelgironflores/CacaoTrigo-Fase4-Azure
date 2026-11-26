using CacaoTrigo.Models;

namespace CacaoTrigo.Services;

public interface IDetallePedidoService
{
    Task<List<DetallePedido>> ObtenerTodosAsync();
    Task<List<DetallePedido>> ObtenerPorPedidosAsync(IReadOnlyCollection<int> pedidoIds);
    Task<DetallePedido?> ObtenerPorIdAsync(int id);
    Task<bool> CrearAsync(DetallePedido detallePedido);
    Task<bool> ActualizarAsync(int id, DetallePedido detallePedido);
    Task<bool> EliminarAsync(int id);
}
