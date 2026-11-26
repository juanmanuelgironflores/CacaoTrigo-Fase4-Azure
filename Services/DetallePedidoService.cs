using CacaoTrigo.Models;
using CacaoTrigo.Repositories;

namespace CacaoTrigo.Services;

public class DetallePedidoService : IDetallePedidoService
{
    private readonly IDetallePedidoRepository _detalleRepository;

    public DetallePedidoService(IDetallePedidoRepository detalleRepository)
    {
        _detalleRepository = detalleRepository;
    }

    public Task<List<DetallePedido>> ObtenerTodosAsync()
    {
        return _detalleRepository.ObtenerTodosAsync();
    }

    public Task<List<DetallePedido>> ObtenerPorPedidosAsync(
        IReadOnlyCollection<int> pedidoIds)
    {
        return _detalleRepository.ObtenerPorPedidosAsync(pedidoIds);
    }

    public Task<DetallePedido?> ObtenerPorIdAsync(int id)
    {
        return _detalleRepository.ObtenerPorIdAsync(id);
    }

    public async Task<bool> CrearAsync(DetallePedido detallePedido)
    {
        if (detallePedido.Cantidad < 1 || detallePedido.Cantidad > 999)
        {
            return false;
        }

        var pedido = await _detalleRepository.ObtenerPedidoPorIdAsync(
            detallePedido.PedidoId);

        var producto = await _detalleRepository.ObtenerProductoPorIdAsync(
            detallePedido.ProductoId);

        if (pedido == null || producto == null)
        {
            return false;
        }

        detallePedido.PrecioUnitario = producto.Precio;
        detallePedido.Subtotal = Math.Round(
            detallePedido.Cantidad * producto.Precio,
            2);

        await _detalleRepository.AgregarAsync(detallePedido);
        await _detalleRepository.GuardarCambiosAsync();
        await _detalleRepository.RecalcularTotalPedidoAsync(detallePedido.PedidoId);

        return true;
    }

    public async Task<bool> ActualizarAsync(
        int id,
        DetallePedido detallePedido)
    {
        if (detallePedido.Cantidad < 1 || detallePedido.Cantidad > 999)
        {
            return false;
        }

        var guardado = await _detalleRepository.ObtenerPorIdAsync(id);

        if (guardado == null)
        {
            return false;
        }

        var pedido = await _detalleRepository.ObtenerPedidoPorIdAsync(
            detallePedido.PedidoId);

        var producto = await _detalleRepository.ObtenerProductoPorIdAsync(
            detallePedido.ProductoId);

        if (pedido == null || producto == null)
        {
            return false;
        }

        var pedidoAnteriorId = guardado.PedidoId;

        guardado.PedidoId = detallePedido.PedidoId;
        guardado.ProductoId = detallePedido.ProductoId;
        guardado.Cantidad = detallePedido.Cantidad;
        guardado.PrecioUnitario = producto.Precio;
        guardado.Subtotal = Math.Round(
            detallePedido.Cantidad * producto.Precio,
            2);

        await _detalleRepository.GuardarCambiosAsync();
        await _detalleRepository.RecalcularTotalPedidoAsync(pedidoAnteriorId);

        if (pedidoAnteriorId != guardado.PedidoId)
        {
            await _detalleRepository.RecalcularTotalPedidoAsync(guardado.PedidoId);
        }

        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var detallePedido = await _detalleRepository.ObtenerPorIdAsync(id);

        if (detallePedido == null)
        {
            return false;
        }

        var pedidoId = detallePedido.PedidoId;

        await _detalleRepository.EliminarAsync(detallePedido);
        await _detalleRepository.GuardarCambiosAsync();
        await _detalleRepository.RecalcularTotalPedidoAsync(pedidoId);

        return true;
    }
}
