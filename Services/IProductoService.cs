using CacaoTrigo.Models;

namespace CacaoTrigo.Services;

public interface IProductoService
{
    Task<List<Producto>> ObtenerTodosAsync();

    Task<List<Producto>> BuscarYOrdenarAsync(
        string? buscar,
        int? categoriaId,
        string? ordenar);

    Task<Producto?> ObtenerPorIdAsync(int id);

    Task<bool> CrearAsync(Producto producto);

    Task<bool> ActualizarAsync(int id, Producto producto);

    Task<bool> EliminarAsync(int id);
}