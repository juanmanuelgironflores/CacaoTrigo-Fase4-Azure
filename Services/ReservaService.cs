using CacaoTrigo.Models;
using CacaoTrigo.Repositories;

namespace CacaoTrigo.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;

    public ReservaService(IReservaRepository reservaRepository)
    {
        _reservaRepository = reservaRepository;
    }

    public Task<List<Reserva>> ObtenerTodosAsync()
    {
        return _reservaRepository.ObtenerTodosAsync();
    }

    public Task<List<Reserva>> ObtenerPorUsuarioAsync(string usuarioId)
    {
        return _reservaRepository.ObtenerPorUsuarioAsync(usuarioId);
    }

    public Task<Reserva?> ObtenerPorIdAsync(int id)
    {
        return _reservaRepository.ObtenerPorIdAsync(id);
    }

    public async Task<bool> CrearAsync(Reserva reserva)
    {
        if (!await DatosValidosAsync(reserva))
        {
            return false;
        }

        await _reservaRepository.AgregarAsync(reserva);
        await _reservaRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> ActualizarAsync(int id, Reserva reserva)
    {
        var reservaGuardada = await _reservaRepository.ObtenerPorIdAsync(id);

        if (reservaGuardada == null ||
            !await DatosValidosAsync(reserva))
        {
            return false;
        }

        reservaGuardada.FechaHora = reserva.FechaHora;
        reservaGuardada.NumeroPersonas = reserva.NumeroPersonas;
        reservaGuardada.NombreContacto = reserva.NombreContacto;
        reservaGuardada.TelefonoContacto = reserva.TelefonoContacto;
        reservaGuardada.CorreoContacto = reserva.CorreoContacto;
        reservaGuardada.Estado = reserva.Estado;
        reservaGuardada.MesaId = reserva.MesaId;
        reservaGuardada.Zona = reserva.Zona;

        await _reservaRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var reserva = await _reservaRepository.ObtenerPorIdAsync(id);

        if (reserva == null)
        {
            return false;
        }

        await _reservaRepository.EliminarAsync(reserva);
        await _reservaRepository.GuardarCambiosAsync();

        return true;
    }

    // Una reserva confirmada necesita mesa; si hay mesa, debe existir.
    private async Task<bool> DatosValidosAsync(Reserva reserva)
    {
        if (reserva.NumeroPersonas < 1)
        {
            return false;
        }

        if (reserva.MesaId.HasValue)
        {
            return await _reservaRepository.MesaExisteAsync(reserva.MesaId.Value);
        }

        return reserva.Estado != "Confirmada";
    }
}
