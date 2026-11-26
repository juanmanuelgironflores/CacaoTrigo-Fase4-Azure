using CacaoTrigo.Models;

namespace CacaoTrigo.Repositories;

public interface ICategoriaRepository
{
    Task<List<Categoria>> ObtenerTodasAsync();
    Task<Categoria?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Categoria categoria);
    Task GuardarCambiosAsync();
    Task EliminarAsync(Categoria categoria);
}