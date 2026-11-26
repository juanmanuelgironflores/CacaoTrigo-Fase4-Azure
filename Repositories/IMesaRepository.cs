using CacaoTrigo.Models;

namespace CacaoTrigo.Repositories;

public interface IMesaRepository
{
    Task<List<Mesa>> ObtenerTodosAsync();
    Task<Mesa?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Mesa mesa);
    Task GuardarCambiosAsync();
    Task EliminarAsync(Mesa mesa);
}