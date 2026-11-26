using CacaoTrigo.Models;
using CacaoTrigo.Repositories;

namespace CacaoTrigo.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProductoService _productoService;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProductoService productoService)
    {
        _pedidoRepository = pedidoRepository;
        _productoService = productoService;
    }

    public Task<List<Pedido>> ObtenerTodosAsync()
    {
        return _pedidoRepository.ObtenerTodosAsync();
    }

    public Task<List<Pedido>> ObtenerPorUsuarioAsync(string usuarioId)
    {
        return _pedidoRepository.ObtenerPorUsuarioAsync(usuarioId);
    }

    public Task<Pedido?> ObtenerPorIdAsync(int id)
    {
        return _pedidoRepository.ObtenerPorIdAsync(id);
    }

    public async Task<bool> CrearAsync(Pedido pedido)
    {
        if (pedido.Fecha == default ||
            string.IsNullOrWhiteSpace(pedido.Estado))
        {
            return false;
        }

        // El total se calcula al agregar productos al pedido.
        pedido.Total = 0m;

        await _pedidoRepository.AgregarAsync(pedido);
        await _pedidoRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> CrearConDetallesAsync(
        Pedido pedido,
        IReadOnlyCollection<DetallePedido> detalles)
    {
        if (pedido.Fecha == default ||
            string.IsNullOrWhiteSpace(pedido.Estado) ||
            detalles.Count == 0 ||
            detalles.Any(d => d.Cantidad < 1 || d.Cantidad > 999))
        {
            return false;
        }

        foreach (var detalle in detalles)
        {
            var producto = await _productoService.ObtenerPorIdAsync(detalle.ProductoId);
            if (producto == null || !producto.Disponible)
            {
                return false;
            }

            detalle.PrecioUnitario = producto.Precio;
            detalle.Subtotal = Math.Round(producto.Precio * detalle.Cantidad, 2);
        }

        pedido.Total = detalles.Sum(d => d.Subtotal);
        await _pedidoRepository.AgregarConDetallesAsync(pedido, detalles);
        return true;
    }

    public async Task<bool> ActualizarAsync(int id, Pedido pedido)
    {
        if (pedido.Fecha == default ||
            string.IsNullOrWhiteSpace(pedido.Estado))
        {
            return false;
        }

        var pedidoGuardado = await _pedidoRepository.ObtenerPorIdAsync(id);

        if (pedidoGuardado == null)
        {
            return false;
        }

        pedidoGuardado.Fecha = pedido.Fecha;
        pedidoGuardado.Estado = pedido.Estado;
        pedidoGuardado.NombreContacto = pedido.NombreContacto;
        pedidoGuardado.TelefonoContacto = pedido.TelefonoContacto;
        pedidoGuardado.DireccionEntrega = pedido.DireccionEntrega;

        // No cambiar Total aquí: se actualiza con los detalles del pedido.
        await _pedidoRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var pedido = await _pedidoRepository.ObtenerPorIdAsync(id);

        if (pedido == null)
        {
            return false;
        }

        await _pedidoRepository.EliminarAsync(pedido);
        await _pedidoRepository.GuardarCambiosAsync();

        return true;
    }
}
