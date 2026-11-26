using CacaoTrigo.Models;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = RolesApp.Administrador)]
public class InsumosController : Controller
{
    private readonly IInsumoService _insumoService;

    public InsumosController(IInsumoService insumoService)
    {
        _insumoService = insumoService;
    }

    // GET: Insumos
    public async Task<IActionResult> Index()
    {
        var insumos = await _insumoService.ObtenerTodosAsync();
        return View(insumos);
    }

    // GET: Insumos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var insumo = await _insumoService.ObtenerPorIdAsync(id.Value);

        if (insumo == null)
        {
            return NotFound();
        }

        return View(insumo);
    }

    // GET: Insumos/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Insumos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Nombre,UnidadMedida,CantidadDisponible")]
        Insumo insumo)
    {
        if (!ModelState.IsValid)
        {
            return View(insumo);
        }

        var creado = await _insumoService.CrearAsync(insumo);

        if (!creado)
        {
            ModelState.AddModelError(
                nameof(insumo.CantidadDisponible),
                "La cantidad disponible no puede ser negativa.");

            return View(insumo);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Insumos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var insumo = await _insumoService.ObtenerPorIdAsync(id.Value);

        if (insumo == null)
        {
            return NotFound();
        }

        return View(insumo);
    }

    // POST: Insumos/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        [Bind("InsumoId,Nombre,UnidadMedida,CantidadDisponible")]
        Insumo insumo)
    {
        if (!ModelState.IsValid)
        {
            return View(insumo);
        }

        var actualizado = await _insumoService.ActualizarAsync(
            insumo.InsumoId,
            insumo);

        if (!actualizado)
        {
            ModelState.AddModelError(
                nameof(insumo.CantidadDisponible),
                "No se pudo actualizar. Verifica que la cantidad no sea negativa.");

            return View(insumo);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Insumos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var insumo = await _insumoService.ObtenerPorIdAsync(id.Value);

        if (insumo == null)
        {
            return NotFound();
        }

        return View(insumo);
    }

    // POST: Insumos/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eliminado = await _insumoService.EliminarAsync(id);

        if (!eliminado)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}