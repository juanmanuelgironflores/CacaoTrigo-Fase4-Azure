using CacaoTrigo.Models;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CacaoTrigo.Controllers;

[Authorize(Roles = RolesApp.Administrador)]
public class DetallesPedidoController : Controller
{
    private readonly IDetallePedidoService _detallePedidoService;
    private readonly IPedidoService _pedidoService;
    private readonly IProductoService _productoService;

    public DetallesPedidoController(
        IDetallePedidoService detallePedidoService,
        IPedidoService pedidoService,
        IProductoService productoService)
    {
        _detallePedidoService = detallePedidoService;
        _pedidoService = pedidoService;
        _productoService = productoService;
    }

    // GET: DetallesPedido
    public async Task<IActionResult> Index()
    {
        var detalles = await _detallePedidoService.ObtenerTodosAsync();
        return View(detalles);
    }

    // GET: DetallesPedido/Details/1
    public async Task<IActionResult> Details(int id)
    {
        var detalle = await _detallePedidoService.ObtenerPorIdAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        return View(detalle);
    }

    // GET: DetallesPedido/Create
    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View();
    }

    // POST: DetallesPedido/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("PedidoId,ProductoId,Cantidad")]
        DetallePedido detallePedido)
    {
        ModelState.Remove(nameof(detallePedido.PrecioUnitario));
        ModelState.Remove(nameof(detallePedido.Subtotal));
        ModelState.Remove(nameof(detallePedido.Pedido));
        ModelState.Remove(nameof(detallePedido.Producto));

        if (ModelState.IsValid)
        {
            var creado = await _detallePedidoService.CrearAsync(detallePedido);

            if (creado)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                string.Empty,
                "No se pudo agregar el producto. Revisa el pedido, el producto y la cantidad.");
        }

        await CargarListasAsync(
            detallePedido.PedidoId,
            detallePedido.ProductoId);

        return View(detallePedido);
    }

    // GET: DetallesPedido/Edit/1
    public async Task<IActionResult> Edit(int id)
    {
        var detalle = await _detallePedidoService.ObtenerPorIdAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        await CargarListasAsync(detalle.PedidoId, detalle.ProductoId);
        return View(detalle);
    }

    // POST: DetallesPedido/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        [Bind("DetallePedidoId,PedidoId,ProductoId,Cantidad")]
        DetallePedido detallePedido)
    {
        ModelState.Remove(nameof(detallePedido.PrecioUnitario));
        ModelState.Remove(nameof(detallePedido.Subtotal));
        ModelState.Remove(nameof(detallePedido.Pedido));
        ModelState.Remove(nameof(detallePedido.Producto));

        if (ModelState.IsValid)
        {
            var actualizado = await _detallePedidoService.ActualizarAsync(
                detallePedido.DetallePedidoId,
                detallePedido);

            if (actualizado)
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        await CargarListasAsync(
            detallePedido.PedidoId,
            detallePedido.ProductoId);

        return View(detallePedido);
    }

    // GET: DetallesPedido/Delete/1
    public async Task<IActionResult> Delete(int id)
    {
        var detalle = await _detallePedidoService.ObtenerPorIdAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        return View(detalle);
    }

    // POST: DetallesPedido/Delete/1
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        [FromForm(Name = "DetallePedidoId")] int id)
    {
        var eliminado = await _detallePedidoService.EliminarAsync(id);

        if (!eliminado)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(
        int? pedidoSeleccionado = null,
        int? productoSeleccionado = null)
    {
        var pedidos = (await _pedidoService.ObtenerTodosAsync())
            .OrderByDescending(p => p.Fecha)
            .Select(p => new
            {
                p.PedidoId,
                Texto = $"Pedido #{p.PedidoId} - {p.Fecha:dd/MM/yyyy HH:mm} - {p.NombreContacto}"
            })
            .ToList();

        ViewBag.PedidoId = new SelectList(
            pedidos,
            "PedidoId",
            "Texto",
            pedidoSeleccionado);

        var productos = (await _productoService.ObtenerTodosAsync())
            .OrderBy(p => p.Nombre)
            .Select(p => new
            {
                p.ProductoId,
                Texto = $"{p.Nombre} - ${p.Precio:F2}"
            })
            .ToList();

        ViewBag.ProductoId = new SelectList(
            productos,
            "ProductoId",
            "Texto",
            productoSeleccionado);
    }
}
