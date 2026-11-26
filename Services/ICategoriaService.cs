using CacaoTrigo.Models;

namespace CacaoTrigo.Services;

public interface ICategoriaService
{
    Task<List<Categoria>> ObtenerTodosAsync();
    Task<Categoria?> ObtenerPorIdAsync(int id);
    Task<bool> CrearAsync(Categoria categoria);
    Task<bool> ActualizarAsync(int id, Categoria categoria);
    Task<bool> EliminarAsync(int id);
}