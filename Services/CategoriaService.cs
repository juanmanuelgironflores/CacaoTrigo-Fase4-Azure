using CacaoTrigo.Models;
using CacaoTrigo.Repositories;
using CacaoTrigo.Repositories;

namespace CacaoTrigo.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaService(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public Task<List<Categoria>> ObtenerTodosAsync()
    {
        return _categoriaRepository.ObtenerTodasAsync();
    }

    public Task<Categoria?> ObtenerPorIdAsync(int id)
    {
        return _categoriaRepository.ObtenerPorIdAsync(id);
    }

    public async Task<bool> CrearAsync(Categoria categoria)
    {
        await _categoriaRepository.AgregarAsync(categoria);
        await _categoriaRepository.GuardarCambiosAsync();
        return true;
    }

    public async Task<bool> ActualizarAsync(int id, Categoria categoria)
    {
        var categoriaGuardada = await _categoriaRepository.ObtenerPorIdAsync(id);

        if (categoriaGuardada == null)
        {
            return false;
        }

        categoriaGuardada.Nombre = categoria.Nombre;
        await _categoriaRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var categoria = await _categoriaRepository.ObtenerPorIdAsync(id);

        if (categoria == null)
        {
            return false;
        }

        await _categoriaRepository.EliminarAsync(categoria);
        await _categoriaRepository.GuardarCambiosAsync();

        return true;
    }
}