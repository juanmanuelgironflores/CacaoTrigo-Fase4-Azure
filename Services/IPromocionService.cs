using CacaoTrigo.Models;

namespace CacaoTrigo.Services;

public interface IPromocionService
{
    Task<List<Promocion>> ObtenerTodosAsync();
    Task<Promocion?> ObtenerPorIdAsync(int id);
    Task<bool> CrearAsync(Promocion promocion);
    Task<bool> ActualizarAsync(int id, Promocion promocion);
    Task<bool> EliminarAsync(int id);
}