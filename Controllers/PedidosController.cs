using CacaoTrigo.Models;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = RolesApp.Administrador)]
public class PedidosController : Controller
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    // GET: Pedidos
    public async Task<IActionResult> Index()
    {
        var pedidos = await _pedidoService.ObtenerTodosAsync();
        return View(pedidos);
    }

    // GET: Pedidos/Details/1
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pedido = await _pedidoService.ObtenerPorIdAsync(id.Value);

        if (pedido == null)
        {
            return NotFound();
        }

        return View(pedido);
    }

    // GET: Pedidos/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Pedidos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Fecha,Estado,NombreContacto,TelefonoContacto,DireccionEntrega")]
        Pedido pedido)
    {
        // El total se calcula desde los productos agregados al pedido.
        pedido.Total = 0m;

        if (pedido.Fecha == default)
        {
            pedido.Fecha = DateTime.Now;
        }

        if (!ModelState.IsValid)
        {
            return View(pedido);
        }

        var creado = await _pedidoService.CrearAsync(pedido);

        if (!creado)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudo guardar el pedido.");

            return View(pedido);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Pedidos/Edit/1
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pedido = await _pedidoService.ObtenerPorIdAsync(id.Value);

        if (pedido == null)
        {
            return NotFound();
        }

        return View(pedido);
    }

    // POST: Pedidos/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        [Bind("PedidoId,Fecha,Estado,NombreContacto,TelefonoContacto,DireccionEntrega")]
        Pedido pedido)
    {
        var pedidoGuardado = await _pedidoService.ObtenerPorIdAsync(pedido.PedidoId);

        if (pedidoGuardado == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            pedido.Total = pedidoGuardado.Total;
            return View(pedido);
        }

        // No permitir que la edición del pedido sobrescriba el total
        // calculado a partir de sus detalles.
        pedido.Total = pedidoGuardado.Total;

        var actualizado = await _pedidoService.ActualizarAsync(
            pedido.PedidoId,
            pedido);

        if (!actualizado)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Pedidos/Delete/1
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pedido = await _pedidoService.ObtenerPorIdAsync(id.Value);

        if (pedido == null)
        {
            return NotFound();
        }

        return View(pedido);
    }

    // POST: Pedidos/Delete/1
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        [FromForm(Name = "PedidoId")] int id)
    {
        var eliminado = await _pedidoService.EliminarAsync(id);

        if (!eliminado)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}