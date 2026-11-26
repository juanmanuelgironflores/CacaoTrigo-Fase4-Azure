using CacaoTrigo.Models;

namespace CacaoTrigo.Repositories;

public interface IInsumoRepository
{
    Task<List<Insumo>> ObtenerTodosAsync();

    Task<Insumo?> ObtenerPorIdAsync(int id);

    Task AgregarAsync(Insumo insumo);

    Task GuardarCambiosAsync();

    Task EliminarAsync(Insumo insumo);
}