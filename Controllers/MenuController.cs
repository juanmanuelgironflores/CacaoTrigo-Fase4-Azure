using CacaoTrigo.Services;
using CacaoTrigo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CacaoTrigo.Controllers;

/// <summary>Menú público: cualquiera puede verlo; solo los clientes pueden pedir.</summary>
public class MenuController : Controller
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaService _categoriaService;

    public MenuController(
        IProductoService productoService,
        ICategoriaService categoriaService)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
    }

    public async Task<IActionResult> Index(int? categoriaId)
    {
        var categorias = await _categoriaService.ObtenerTodosAsync();
        var productos = (await _productoService.ObtenerTodosAsync())
            .Where(p => p.Disponible &&
                (!categoriaId.HasValue || p.CategoriaId == categoriaId.Value))
            .OrderBy(p => p.Categoria?.Nombre)
            .ThenBy(p => p.Nombre)
            .ToList();

        return View(new MenuViewModel
        {
            Categorias = categorias,
            Productos = productos,
            CategoriaSeleccionada = categoriaId
        });
    }
}
