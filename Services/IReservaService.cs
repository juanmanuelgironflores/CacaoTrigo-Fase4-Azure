using CacaoTrigo.Models;

namespace CacaoTrigo.Services;

public interface IReservaService
{
    Task<List<Reserva>> ObtenerTodosAsync();
    Task<List<Reserva>> ObtenerPorUsuarioAsync(string usuarioId);
    Task<Reserva?> ObtenerPorIdAsync(int id);
    Task<bool> CrearAsync(Reserva reserva);
    Task<bool> ActualizarAsync(int id, Reserva reserva);
    Task<bool> EliminarAsync(int id);
}
