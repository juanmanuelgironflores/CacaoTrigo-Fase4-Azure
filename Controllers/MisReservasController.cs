using System.Globalization;
using CacaoTrigo.Models;
using CacaoTrigo.Models.ViewModels;
using CacaoTrigo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CacaoTrigo.Controllers;

/// <summary>Reservas de mesa del cliente que inició sesión.</summary>
[Authorize(Roles = RolesApp.Cliente)]
public class MisReservasController : Controller
{
    private readonly IReservaService _reservaService;
    private readonly UserManager<ApplicationUser> _usuarios;

    public MisReservasController(
        IReservaService reservaService,
        UserManager<ApplicationUser> usuarios)
    {
        _reservaService = reservaService;
        _usuarios = usuarios;
    }

    public async Task<IActionResult> Index()
    {
        var usuarioId = _usuarios.GetUserId(User);

        var reservas = await _reservaService.ObtenerPorUsuarioAsync(usuarioId!);

        return View(reservas);
    }

    [HttpGet]
    public async Task<IActionResult> Crear(string? zona = null)
    {
        var usuario = await _usuarios.GetUserAsync(User);

        var modelo = new ReservaClienteViewModel
        {
            Zona = ZonasReserva.EsValida(zona) ? zona! : ZonasReserva.Interior,
            NombreContacto = usuario?.NombreCompleto ?? string.Empty,
            TelefonoContacto = usuario?.PhoneNumber ?? string.Empty
        };

        CargarOpciones();
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ReservaClienteViewModel modelo)
    {
        if (!ZonasReserva.EsValida(modelo.Zona))
        {
            ModelState.AddModelError(nameof(modelo.Zona), "Elige Terraza o Interior.");
        }

        DateTime? fechaHora = null;

        if (modelo.Fecha.HasValue && !string.IsNullOrEmpty(modelo.Hora))
        {
            if (HorarioReservas.Franjas().Contains(modelo.Hora) &&
                TimeSpan.TryParseExact(
                    modelo.Hora,
                    @"hh\:mm",
                    CultureInfo.InvariantCulture,
                    out var hora))
            {
                fechaHora = modelo.Fecha.Value.Date + hora;
            }
            else
            {
                ModelState.AddModelError(nameof(modelo.Hora), "Elige una hora de la lista.");
            }
        }

        if (fechaHora.HasValue)
        {
            var ahora = DateTime.Now;

            if (fechaHora.Value < ahora.AddMinutes(HorarioReservas.MinutosMinimosDeAnticipacion))
            {
                ModelState.AddModelError(
                    nameof(modelo.Fecha),
                    $"Elige una fecha y hora futuras (con al menos {HorarioReservas.MinutosMinimosDeAnticipacion} minutos de anticipación).");
            }
            else if (fechaHora.Value > ahora.AddDays(HorarioReservas.DiasMaximosDeAnticipacion))
            {
                ModelState.AddModelError(
                    nameof(modelo.Fecha),
                    $"Solo puedes reservar con hasta {HorarioReservas.DiasMaximosDeAnticipacion} días de anticipación.");
            }
        }

        if (!ModelState.IsValid || !fechaHora.HasValue)
        {
            CargarOpciones();
            return View(modelo);
        }

        var usuario = await _usuarios.GetUserAsync(User);

        var reserva = new Reserva
        {
            FechaHora = fechaHora.Value,
            NumeroPersonas = modelo.NumeroPersonas!.Value,
            NombreContacto = modelo.NombreContacto.Trim(),
            TelefonoContacto = modelo.TelefonoContacto.Trim(),
            CorreoContacto = usuario?.Email,
            Estado = "Pendiente",
            Zona = modelo.Zona,
            UsuarioId = usuario?.Id
        };

        var creada = await _reservaService.CrearAsync(reserva);

        if (!creada)
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar la reserva.");
            CargarOpciones();
            return View(modelo);
        }

        TempData["Mensaje"] =
            $"Recibimos tu reserva en {modelo.Zona.ToLowerInvariant()} para el " +
            $"{fechaHora.Value:dd/MM/yyyy} a las {fechaHora.Value:HH:mm}. " +
            "Te confirmaremos la mesa muy pronto.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var usuarioId = _usuarios.GetUserId(User);

        // Solo puede cancelar sus propias reservas.
        var reserva = (await _reservaService.ObtenerPorUsuarioAsync(usuarioId!))
            .FirstOrDefault(r => r.ReservaId == id);

        if (reserva == null)
        {
            return NotFound();
        }

        if (reserva.Estado != "Cancelada" && reserva.FechaHora > DateTime.Now)
        {
            reserva.Estado = "Cancelada";
            if (await _reservaService.ActualizarAsync(id, reserva))
            {
                TempData["Mensaje"] = "Tu reserva se canceló.";
            }
        }

        return RedirectToAction(nameof(Index));
    }

    private void CargarOpciones()
    {
        ViewBag.Franjas = HorarioReservas.Franjas();
        ViewBag.FechaMinima = DateTime.Today.ToString("yyyy-MM-dd");
        ViewBag.FechaMaxima = DateTime.Today
            .AddDays(HorarioReservas.DiasMaximosDeAnticipacion)
            .ToString("yyyy-MM-dd");
    }
}
