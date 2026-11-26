using CacaoTrigo.Helpers;
using CacaoTrigo.Models;
using CacaoTrigo.Models.ViewModels;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CacaoTrigo.Controllers;

[Authorize(Roles = RolesApp.Cliente)]
public class CarritoController : Controller
{
    private const int CantidadMaxima = 99;

    private readonly IProductoService _productoService;
    private readonly IPedidoService _pedidoService;
    private readonly UserManager<ApplicationUser> _usuarios;

    public CarritoController(
        IProductoService productoService,
        IPedidoService pedidoService,
        UserManager<ApplicationUser> usuarios)
    {
        _productoService = productoService;
        _pedidoService = pedidoService;
        _usuarios = usuarios;
    }

    public async Task<IActionResult> Index()
    {
        var modelo = await ConstruirAsync();

        var usuario = await _usuarios.GetUserAsync(User);
        modelo.Datos.NombreContacto = usuario?.NombreCompleto ?? string.Empty;
        modelo.Datos.TelefonoContacto = usuario?.PhoneNumber ?? string.Empty;

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar(int productoId, int cantidad = 1, string? returnUrl = null)
    {
        var producto = await _productoService.ObtenerPorIdAsync(productoId);

        if (producto == null || !producto.Disponible)
        {
            TempData["Error"] = "Ese producto ya no está disponible.";
            return Volver(returnUrl);
        }

        cantidad = Math.Clamp(cantidad, 1, CantidadMaxima);

        var carrito = CarritoSesion.Obtener(HttpContext.Session);
        carrito.TryGetValue(productoId, out var actual);
        carrito[productoId] = Math.Min(CantidadMaxima, actual + cantidad);
        CarritoSesion.Guardar(HttpContext.Session, carrito);

        TempData["Mensaje"] = $"{producto.Nombre} se agregó a tu pedido.";
        return Volver(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Actualizar(int productoId, int cantidad)
    {
        var carrito = CarritoSesion.Obtener(HttpContext.Session);

        if (carrito.ContainsKey(productoId))
        {
            if (cantidad <= 0)
            {
                carrito.Remove(productoId);
            }
            else
            {
                carrito[productoId] = Math.Min(CantidadMaxima, cantidad);
            }

            CarritoSesion.Guardar(HttpContext.Session, carrito);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Quitar(int productoId)
    {
        var carrito = CarritoSesion.Obtener(HttpContext.Session);

        if (carrito.Remove(productoId))
        {
            CarritoSesion.Guardar(HttpContext.Session, carrito);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // El parámetro se llama "datos": enlaza los campos "Datos.NombreContacto", etc.
    public async Task<IActionResult> Confirmar(DatosPedidoViewModel datos)
    {
        var modelo = await ConstruirAsync();

        if (!modelo.Lineas.Any())
        {
            TempData["Error"] = "Tu pedido está vacío. Elige algo del menú.";
            return RedirectToAction("Index", "Menu");
        }

        if (datos.Modalidad != DatosPedidoViewModel.Recoger &&
            datos.Modalidad != DatosPedidoViewModel.Domicilio)
        {
            ModelState.AddModelError("Datos.Modalidad", "Elige una modalidad válida.");
        }

        if (datos.Modalidad == DatosPedidoViewModel.Domicilio &&
            string.IsNullOrWhiteSpace(datos.DireccionEntrega))
        {
            ModelState.AddModelError(
                "Datos.DireccionEntrega",
                "Escribe la dirección donde entregaremos tu pedido.");
        }

        if (!ModelState.IsValid)
        {
            modelo.Datos = datos;
            return View("Index", modelo);
        }

        var pedido = new Pedido
        {
            Fecha = DateTime.Now,
            Estado = "Pendiente",
            Total = modelo.Total,
            NombreContacto = datos.NombreContacto.Trim(),
            TelefonoContacto = datos.TelefonoContacto.Trim(),
            DireccionEntrega = datos.Modalidad == DatosPedidoViewModel.Domicilio
                ? datos.DireccionEntrega!.Trim()
                : "Recoger en el local",
            UsuarioId = _usuarios.GetUserId(User)
        };

        var detalles = modelo.Lineas.Select(linea => new DetallePedido
        {
            ProductoId = linea.ProductoId,
            Cantidad = linea.Cantidad
        }).ToList();

        var creado = await _pedidoService.CrearConDetallesAsync(pedido, detalles);

        if (!creado)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudo confirmar el pedido. Revisa que los productos sigan disponibles.");
            modelo.Datos = datos;
            return View("Index", modelo);
        }

        CarritoSesion.Vaciar(HttpContext.Session);

        TempData["Mensaje"] = "¡Recibimos tu pedido! Lo puedes ver en tu historial.";
        return RedirectToAction("Index", "MisPedidos");
    }

    // Arma el carrito con los datos actuales de la base (precio y disponibilidad).
    private async Task<CarritoViewModel> ConstruirAsync()
    {
        var carrito = CarritoSesion.Obtener(HttpContext.Session);
        var ids = carrito.Keys.ToList();

        var productos = ids.Count == 0
            ? new List<Producto>()
            : (await _productoService.ObtenerTodosAsync())
                .Where(p => ids.Contains(p.ProductoId) && p.Disponible)
                .ToList();

        // Quita de la sesión lo que ya no existe o dejó de estar disponible.
        if (productos.Count != carrito.Count)
        {
            carrito = productos.ToDictionary(p => p.ProductoId, p => carrito[p.ProductoId]);
            CarritoSesion.Guardar(HttpContext.Session, carrito);
        }

        var modelo = new CarritoViewModel();

        foreach (var producto in productos.OrderBy(p => p.Nombre))
        {
            modelo.Lineas.Add(new LineaCarrito
            {
                ProductoId = producto.ProductoId,
                Nombre = producto.Nombre,
                ImagenUrl = producto.ImagenUrl,
                PrecioUnitario = producto.Precio,
                Cantidad = carrito[producto.ProductoId]
            });
        }

        return modelo;
    }

    private IActionResult Volver(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Menu");
    }
}
