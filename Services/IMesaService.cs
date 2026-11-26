using CacaoTrigo.Models;

namespace CacaoTrigo.Services;

public interface IMesaService
{
    Task<List<Mesa>> ObtenerTodosAsync();
    Task<Mesa?> ObtenerPorIdAsync(int id);
    Task<bool> CrearAsync(Mesa mesa);
    Task<bool> ActualizarAsync(int id, Mesa mesa);
    Task<bool> EliminarAsync(int id);
}