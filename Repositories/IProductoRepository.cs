using CacaoTrigo.Models;

namespace CacaoTrigo.Repositories;

public interface IProductoRepository
{
    Task<List<Producto>> ObtenerTodosAsync();

    Task<List<Producto>> BuscarYOrdenarAsync(
        string? buscar,
        int? categoriaId,
        string? ordenar);

    Task<Producto?> ObtenerPorIdAsync(int id);

    Task<bool> CategoriaExisteAsync(int categoriaId);

    Task AgregarAsync(Producto producto);

    Task GuardarCambiosAsync();

    Task EliminarAsync(Producto producto);
}