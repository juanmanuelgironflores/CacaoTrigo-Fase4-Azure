using CacaoTrigo.Models;

namespace CacaoTrigo.Services;

public interface IInsumoService
{
    Task<List<Insumo>> ObtenerTodosAsync();
    Task<Insumo?> ObtenerPorIdAsync(int id);
    Task<bool> CrearAsync(Insumo insumo);
    Task<bool> ActualizarAsync(int id, Insumo insumo);
    Task<bool> EliminarAsync(int id);
}