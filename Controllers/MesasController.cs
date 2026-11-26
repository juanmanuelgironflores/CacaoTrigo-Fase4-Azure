using CacaoTrigo.Models;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CacaoTrigo.Controllers;

[Authorize(Roles = RolesApp.Administrador)]
public class MesasController : Controller
{
    private readonly IMesaService _mesaService;

    public MesasController(IMesaService mesaService)
    {
        _mesaService = mesaService;
    }

    // GET: Mesas
    public async Task<IActionResult> Index()
    {
        var mesas = await _mesaService.ObtenerTodosAsync();
        return View(mesas);
    }

    // GET: Mesas/Details/1
    public async Task<IActionResult> Details(int id)
    {
        var mesa = await _mesaService.ObtenerPorIdAsync(id);

        if (mesa == null)
        {
            return NotFound();
        }

        return View(mesa);
    }

    // GET: Mesas/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Mesas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Numero,Capacidad,Ubicacion,Accesible")] Mesa mesa)
    {
        if (!ModelState.IsValid)
        {
            return View(mesa);
        }

        var creada = await _mesaService.CrearAsync(mesa);

        if (!creada)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudo crear la mesa. Verifica el número y la capacidad.");
            return View(mesa);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Mesas/Edit/1
    public async Task<IActionResult> Edit(int id)
    {
        var mesa = await _mesaService.ObtenerPorIdAsync(id);

        if (mesa == null)
        {
            return NotFound();
        }

        return View(mesa);
    }

    // POST: Mesas/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        [Bind("MesaId,Numero,Capacidad,Ubicacion,Accesible")] Mesa mesa)
    {
        if (!ModelState.IsValid)
        {
            return View(mesa);
        }

        var actualizada = await _mesaService.ActualizarAsync(
            mesa.MesaId,
            mesa);

        if (!actualizada)
        {
            var mesaExiste = await _mesaService.ObtenerPorIdAsync(mesa.MesaId);

            if (mesaExiste == null)
            {
                return NotFound();
            }

            ModelState.AddModelError(
                string.Empty,
                "No se pudo actualizar la mesa. Verifica el número y la capacidad.");
            return View(mesa);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Mesas/Delete/1
    public async Task<IActionResult> Delete(int id)
    {
        var mesa = await _mesaService.ObtenerPorIdAsync(id);

        if (mesa == null)
        {
            return NotFound();
        }

        return View(mesa);
    }

    // POST: Mesas/Delete/1
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        [FromForm(Name = "MesaId")] int id)
    {
        var eliminada = await _mesaService.EliminarAsync(id);

        if (!eliminada)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}