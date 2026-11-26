using CacaoTrigo.Models;

namespace CacaoTrigo.Repositories;

public interface IPedidoRepository
{
    Task<List<Pedido>> ObtenerTodosAsync();
    Task<List<Pedido>> ObtenerPorUsuarioAsync(string usuarioId);
    Task<Pedido?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Pedido pedido);
    Task AgregarConDetallesAsync(Pedido pedido, IReadOnlyCollection<DetallePedido> detalles);
    Task GuardarCambiosAsync();
    Task EliminarAsync(Pedido pedido);
}
