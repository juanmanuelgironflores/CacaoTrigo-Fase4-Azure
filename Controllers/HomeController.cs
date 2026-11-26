using CacaoTrigo.Models;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CacaoTrigo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductoService _productoService;

        public HomeController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public async Task<IActionResult> Index()
        {
            List<Producto> destacados;

            try
            {
                var productos = await _productoService.ObtenerTodosAsync();
                destacados = productos
                    .Where(p => p.Disponible)
                    .Take(4)
                    .ToList();
            }
            catch (Exception)
            {
                // La portada debe cargar aunque la base de datos no responda.
                destacados = new List<Producto>();
            }

            return View(destacados);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
