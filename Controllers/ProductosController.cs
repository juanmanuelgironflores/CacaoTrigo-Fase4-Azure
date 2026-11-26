using CacaoTrigo.Models;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = RolesApp.Administrador)]
public class ProductosController : Controller
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaService _categoriaService;
    private readonly IImagenService _imagenService;

    public ProductosController(
        IProductoService productoService,
        ICategoriaService categoriaService,
        IImagenService imagenService)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
        _imagenService = imagenService;
    }

    // GET: Productos
    public async Task<IActionResult> Index(
        string? buscar,
        int? categoriaId,
        string? ordenar)
    {
        var productos = await _productoService.BuscarYOrdenarAsync(
            buscar,
            categoriaId,
            ordenar);

        var categorias = await _categoriaService.ObtenerTodosAsync();

        ViewBag.Categorias = new SelectList(
            categorias,
            "CategoriaId",
            "Nombre",
            categoriaId);

        ViewBag.BuscarActual = buscar;
        ViewBag.CategoriaActual = categoriaId;
        ViewBag.OrdenActual = ordenar;

        return View(productos);
    }

    // GET: Productos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var producto = await _productoService.ObtenerPorIdAsync(id.Value);

        if (producto == null)
        {
            return NotFound();
        }

        return View(producto);
    }

    // GET: Productos/Create
    public async Task<IActionResult> Create()
    {
        await CargarCategoriasAsync();
        return View();
    }

    // POST: Productos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Nombre,Descripcion,Precio,Disponible,CategoriaId")]
        Producto producto,
        IFormFile? imagen)
    {
        if (await _categoriaService.ObtenerPorIdAsync(producto.CategoriaId) == null)
        {
            ModelState.AddModelError(
                nameof(producto.CategoriaId),
                "Selecciona una categoría válida.");
        }

        // La imagen se guarda solo si el resto del formulario es válido.
        if (ModelState.IsValid && imagen is { Length: > 0 })
        {
            var resultado = await _imagenService.GuardarProductoAsync(imagen);

            if (resultado.Exitoso)
            {
                producto.ImagenUrl = resultado.Url;
            }
            else
            {
                ModelState.AddModelError("imagen", resultado.Error!);
            }
        }

        if (ModelState.IsValid)
        {
            var creado = await _productoService.CrearAsync(producto);

            if (creado)
            {
                return RedirectToAction(nameof(Index));
            }

            _imagenService.EliminarSiEsLocal(producto.ImagenUrl);
            producto.ImagenUrl = null;

            ModelState.AddModelError(
                string.Empty,
                "No se pudo guardar el producto.");
        }

        await CargarCategoriasAsync(producto.CategoriaId);
        return View(producto);
    }

    // GET: Productos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var producto = await _productoService.ObtenerPorIdAsync(id.Value);

        if (producto == null)
        {
            return NotFound();
        }

        await CargarCategoriasAsync(producto.CategoriaId);
        return View(producto);
    }

    // POST: Productos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("ProductoId,Nombre,Descripcion,Precio,Disponible,CategoriaId")]
        Producto producto,
        IFormFile? imagen,
        bool quitarImagen)
    {
        if (id == null || id != producto.ProductoId)
        {
            return NotFound();
        }

        var existente = await _productoService.ObtenerPorIdAsync(id.Value);

        if (existente == null)
        {
            return NotFound();
        }

        // La imagen actual viene de la base de datos, no del formulario.
        var imagenAnterior = existente.ImagenUrl;
        producto.ImagenUrl = imagenAnterior;

        if (await _categoriaService.ObtenerPorIdAsync(producto.CategoriaId) == null)
        {
            ModelState.AddModelError(
                nameof(producto.CategoriaId),
                "Selecciona una categoría válida.");
        }

        string? imagenNueva = null;

        if (ModelState.IsValid && imagen is { Length: > 0 })
        {
            var resultado = await _imagenService.GuardarProductoAsync(imagen);

            if (resultado.Exitoso)
            {
                imagenNueva = resultado.Url;
                producto.ImagenUrl = imagenNueva;
            }
            else
            {
                ModelState.AddModelError("imagen", resultado.Error!);
            }
        }
        else if (quitarImagen)
        {
            producto.ImagenUrl = null;
        }

        if (ModelState.IsValid)
        {
            var actualizado = await _productoService.ActualizarAsync(
                producto.ProductoId,
                producto);

            if (actualizado)
            {
                if (producto.ImagenUrl != imagenAnterior)
                {
                    _imagenService.EliminarSiEsLocal(imagenAnterior);
                }

                return RedirectToAction(nameof(Index));
            }

            _imagenService.EliminarSiEsLocal(imagenNueva);
            return NotFound();
        }

        // Al volver a mostrar el formulario, se muestra la imagen que sigue guardada.
        producto.ImagenUrl = imagenAnterior;
        await CargarCategoriasAsync(producto.CategoriaId);
        return View(producto);
    }

    // GET: Productos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var producto = await _productoService.ObtenerPorIdAsync(id.Value);

        if (producto == null)
        {
            return NotFound();
        }

        return View(producto);
    }

    // POST: Productos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int productoId)
    {
        var producto = await _productoService.ObtenerPorIdAsync(productoId);
        var imagen = producto?.ImagenUrl;

        var eliminado = await _productoService.EliminarAsync(productoId);

        if (!eliminado)
        {
            return NotFound();
        }

        _imagenService.EliminarSiEsLocal(imagen);

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarCategoriasAsync(
        int? categoriaSeleccionada = null)
    {
        var categorias = await _categoriaService.ObtenerTodosAsync();

        ViewBag.CategoriaId = new SelectList(
            categorias,
            "CategoriaId",
            "Nombre",
            categoriaSeleccionada);
    }
}
