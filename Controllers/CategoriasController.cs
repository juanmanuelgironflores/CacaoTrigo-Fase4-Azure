using CacaoTrigo.Models;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = RolesApp.Administrador)]
public class CategoriasController : Controller
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    public async Task<IActionResult> Index()
    {
        var categorias = await _categoriaService.ObtenerTodosAsync();
        return View(categorias);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var categoria = await _categoriaService.ObtenerPorIdAsync(id.Value);

        if (categoria == null)
        {
            return NotFound();
        }

        return View(categoria);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Nombre")] Categoria categoria)
    {
        if (!ModelState.IsValid)
        {
            return View(categoria);
        }

        var creada = await _categoriaService.CrearAsync(categoria);

        if (!creada)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudo guardar la categoría.");

            return View(categoria);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var categoria = await _categoriaService.ObtenerPorIdAsync(id.Value);

        if (categoria == null)
        {
            return NotFound();
        }

        return View(categoria);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("CategoriaId,Nombre")] Categoria categoria)
    {
        if (id != categoria.CategoriaId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(categoria);
        }

        var actualizada = await _categoriaService.ActualizarAsync(id, categoria);

        if (!actualizada)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var categoria = await _categoriaService.ObtenerPorIdAsync(id.Value);

        if (categoria == null)
        {
            return NotFound();
        }

        return View(categoria);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eliminada = await _categoriaService.EliminarAsync(id);

        if (!eliminada)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}