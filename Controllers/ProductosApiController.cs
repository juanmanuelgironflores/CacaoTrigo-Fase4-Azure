using System.Linq;
using System.ComponentModel.DataAnnotations;
using CacaoTrigo.Models;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/productos")]
[Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Roles = RolesApp.Administrador)]
public class ProductosApiController : ControllerBase
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaService _categoriaService;
    private readonly IImagenService _imagenService;

    public ProductosApiController(
        IProductoService productoService,
        ICategoriaService categoriaService,
        IImagenService imagenService)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
        _imagenService = imagenService;
    }

    // GET: api/productos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoApiResponse>>> ObtenerTodos()
    {
        var productos = await _productoService.ObtenerTodosAsync();
        return Ok(productos.Select(ConvertirRespuesta).ToList());
    }

    // GET: api/productos/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductoApiResponse>> ObtenerPorId(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);

        if (producto == null)
        {
            return NotFound();
        }

        return Ok(ConvertirRespuesta(producto));
    }

    // POST: api/productos
    [HttpPost]
    public async Task<ActionResult<ProductoApiResponse>> Crear(
        [FromBody] ProductoApiRequest solicitud)
    {
        var categoriaExiste =
            await _categoriaService.ObtenerPorIdAsync(solicitud.CategoriaId) != null;

        if (!categoriaExiste)
        {
            return BadRequest(new { mensaje = "La categoría no existe." });
        }

        var producto = new Producto
        {
            Nombre = solicitud.Nombre,
            Descripcion = solicitud.Descripcion,
            Precio = solicitud.Precio,
            Disponible = solicitud.Disponible,
            CategoriaId = solicitud.CategoriaId
        };

        var creado = await _productoService.CrearAsync(producto);

        if (!creado)
        {
            return Conflict(new { mensaje = "No se pudo crear el producto." });
        }

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = producto.ProductoId },
            ConvertirRespuesta(producto));
    }

    // PUT: api/productos/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] ProductoApiRequest solicitud)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);

        if (producto == null)
        {
            return NotFound();
        }

        var categoriaExiste =
            await _categoriaService.ObtenerPorIdAsync(solicitud.CategoriaId) != null;

        if (!categoriaExiste)
        {
            return BadRequest(new { mensaje = "La categoría no existe." });
        }

        producto.Nombre = solicitud.Nombre;
        producto.Descripcion = solicitud.Descripcion;
        producto.Precio = solicitud.Precio;
        producto.Disponible = solicitud.Disponible;
        producto.CategoriaId = solicitud.CategoriaId;

        var actualizado = await _productoService.ActualizarAsync(id, producto);

        if (!actualizado)
        {
            return Conflict(new { mensaje = "No se pudo actualizar el producto." });
        }

        return NoContent();
    }

    // DELETE: api/productos/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var producto = await _productoService.ObtenerPorIdAsync(id);

        if (producto == null)
        {
            return NotFound();
        }

        var eliminado = await _productoService.EliminarAsync(id);

        if (!eliminado)
        {
            return Conflict(new { mensaje = "No se pudo eliminar el producto." });
        }

        _imagenService.EliminarSiEsLocal(producto.ImagenUrl);
        return NoContent();
    }

    private static ProductoApiResponse ConvertirRespuesta(Producto producto)
    {
        return new ProductoApiResponse(
            producto.ProductoId,
            producto.Nombre,
            producto.Descripcion,
            producto.Precio,
            producto.Disponible,
            producto.CategoriaId,
            producto.ImagenUrl);
    }
}

public sealed record ProductoApiResponse(
    int ProductoId,
    string Nombre,
    string? Descripcion,
    decimal Precio,
    bool Disponible,
    int CategoriaId,
    string? ImagenUrl);

public sealed class ProductoApiRequest
{
    [Required]
    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal Precio { get; set; }

    public bool Disponible { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoriaId { get; set; }
}
