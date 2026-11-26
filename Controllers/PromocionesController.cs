using CacaoTrigo.Models;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = RolesApp.Administrador)]
public class PromocionesController : Controller
{
    private readonly IPromocionService _promocionService;

    public PromocionesController(IPromocionService promocionService)
    {
        _promocionService = promocionService;
    }

    // GET: Promociones
    public async Task<IActionResult> Index()
    {
        var promociones = await _promocionService.ObtenerTodosAsync();
        return View(promociones);
    }

    // GET: Promociones/Details/1
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var promocion = await _promocionService.ObtenerPorIdAsync(id.Value);

        if (promocion == null)
        {
            return NotFound();
        }

        return View(promocion);
    }

    // GET: Promociones/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Promociones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Nombre,Descripcion,ValorDescuento,EsPorcentaje,FechaInicio,FechaFin,Condiciones,Activa")]
        Promocion promocion)
    {
        if (!ModelState.IsValid)
        {
            return View(promocion);
        }

        var creada = await _promocionService.CrearAsync(promocion);

        if (!creada)
        {
            ModelState.AddModelError(
                string.Empty,
                "Revisa el descuento y las fechas de la promoción.");
            return View(promocion);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Promociones/Edit/1
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var promocion = await _promocionService.ObtenerPorIdAsync(id.Value);

        if (promocion == null)
        {
            return NotFound();
        }

        return View(promocion);
    }

    // POST: Promociones/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("PromocionId,Nombre,Descripcion,ValorDescuento,EsPorcentaje,FechaInicio,FechaFin,Condiciones,Activa")]
        Promocion promocion)
    {
        if (id != promocion.PromocionId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(promocion);
        }

        var actualizada = await _promocionService.ActualizarAsync(id, promocion);

        if (!actualizada)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudo actualizar. Revisa el descuento y las fechas.");
            return View(promocion);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Promociones/Delete/1
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var promocion = await _promocionService.ObtenerPorIdAsync(id.Value);

        if (promocion == null)
        {
            return NotFound();
        }

        return View(promocion);
    }

    // POST: Promociones/Delete/1
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eliminada = await _promocionService.EliminarAsync(id);

        if (!eliminada)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}