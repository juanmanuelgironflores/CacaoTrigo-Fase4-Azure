using CacaoTrigo.Models;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CacaoTrigo.Controllers;

[Authorize(Roles = RolesApp.Administrador)]
public class ReservasController : Controller
{
    private readonly IReservaService _reservaService;
    private readonly IMesaService _mesaService;

    public ReservasController(
        IReservaService reservaService,
        IMesaService mesaService)
    {
        _reservaService = reservaService;
        _mesaService = mesaService;
    }

    public async Task<IActionResult> Index()
    {
        var reservas = await _reservaService.ObtenerTodosAsync();
        return View(reservas);
    }

    public async Task<IActionResult> Details(int id)
    {
        var reserva = await _reservaService.ObtenerPorIdAsync(id);

        if (reserva == null)
        {
            return NotFound();
        }

        return View(reserva);
    }

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("FechaHora,NumeroPersonas,NombreContacto,TelefonoContacto,CorreoContacto,Estado,Zona,MesaId")]
        Reserva reserva)
    {
        if (ModelState.IsValid)
        {
            var creada = await _reservaService.CrearAsync(reserva);

            if (creada)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                string.Empty,
                "Revisa el número de personas y la mesa seleccionada. Una reserva confirmada necesita una mesa.");
        }

        await CargarListasAsync(reserva.MesaId, reserva.Estado, reserva.Zona);
        return View(reserva);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var reserva = await _reservaService.ObtenerPorIdAsync(id);

        if (reserva == null)
        {
            return NotFound();
        }

        await CargarListasAsync(reserva.MesaId, reserva.Estado, reserva.Zona);
        return View(reserva);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("ReservaId,FechaHora,NumeroPersonas,NombreContacto,TelefonoContacto,CorreoContacto,Estado,Zona,MesaId")]
        Reserva reserva)
    {
        if (id != reserva.ReservaId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var actualizada = await _reservaService.ActualizarAsync(id, reserva);

            if (actualizada)
            {
                return RedirectToAction(nameof(Index));
            }

            if (await _reservaService.ObtenerPorIdAsync(id) == null)
            {
                return NotFound();
            }

            ModelState.AddModelError(
                string.Empty,
                "Revisa el número de personas y la mesa seleccionada. Una reserva confirmada necesita una mesa.");
        }

        await CargarListasAsync(reserva.MesaId, reserva.Estado, reserva.Zona);
        return View(reserva);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var reserva = await _reservaService.ObtenerPorIdAsync(id);

        if (reserva == null)
        {
            return NotFound();
        }

        return View(reserva);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        [FromForm(Name = "ReservaId")] int id)
    {
        var eliminada = await _reservaService.EliminarAsync(id);

        if (!eliminada)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(
        int? mesaId = null,
        string? estado = null,
        string? zona = null)
    {
        var mesas = (await _mesaService.ObtenerTodosAsync())
            .OrderBy(m => m.Numero)
            .Select(m => new
            {
                m.MesaId,
                Texto = $"Mesa {m.Numero} - {m.Ubicacion}"
            })
            .ToList();

        ViewBag.MesaId = new SelectList(
            mesas,
            "MesaId",
            "Texto",
            mesaId);

        var estados = new[]
        {
            "Pendiente",
            "Confirmada",
            "Cancelada"
        };

        ViewBag.Estado = new SelectList(estados, estado);
        ViewBag.Zona = new SelectList(ZonasReserva.Todas, zona);
    }
}