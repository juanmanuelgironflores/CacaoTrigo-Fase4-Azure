using CacaoTrigo.Models;
using CacaoTrigo.Models.ViewModels;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CacaoTrigo.Controllers;

/// <summary>Historial de compras del cliente que inició sesión.</summary>
[Authorize(Roles = RolesApp.Cliente)]
public class MisPedidosController : Controller
{
    private readonly IPedidoService _pedidoService;
    private readonly IDetallePedidoService _detallePedidoService;
    private readonly UserManager<ApplicationUser> _usuarios;

    public MisPedidosController(
        IPedidoService pedidoService,
        IDetallePedidoService detallePedidoService,
        UserManager<ApplicationUser> usuarios)
    {
        _pedidoService = pedidoService;
        _detallePedidoService = detallePedidoService;
        _usuarios = usuarios;
    }

    public async Task<IActionResult> Index()
    {
        var usuarioId = _usuarios.GetUserId(User);

        var pedidos = await _pedidoService.ObtenerPorUsuarioAsync(usuarioId!);

        var ids = pedidos.Select(p => p.PedidoId).ToList();

        var detalles = await _detallePedidoService.ObtenerPorPedidosAsync(ids);

        var historial = pedidos
            .Select(p => new PedidoHistorialViewModel
            {
                Pedido = p,
                Detalles = detalles.Where(d => d.PedidoId == p.PedidoId).ToList()
            })
            .ToList();

        return View(historial);
    }
}
