using CacaoTrigo.Models;

namespace CacaoTrigo.Repositories;

public interface IPromocionRepository
{
    Task<List<Promocion>> ObtenerTodosAsync();
    Task<Promocion?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Promocion promocion);
    Task GuardarCambiosAsync();
    Task EliminarAsync(Promocion promocion);
}