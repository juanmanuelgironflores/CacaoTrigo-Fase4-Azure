using CacaoTrigo.Models;
using CacaoTrigo.Repositories;

namespace CacaoTrigo.Services;

public class InsumoService : IInsumoService
{
    private readonly IInsumoRepository _insumoRepository;

    public InsumoService(IInsumoRepository insumoRepository)
    {
        _insumoRepository = insumoRepository;
    }

    public Task<List<Insumo>> ObtenerTodosAsync()
    {
        return _insumoRepository.ObtenerTodosAsync();
    }

    public Task<Insumo?> ObtenerPorIdAsync(int id)
    {
        return _insumoRepository.ObtenerPorIdAsync(id);
    }

    public async Task<bool> CrearAsync(Insumo insumo)
    {
        if (insumo.CantidadDisponible < 0)
        {
            return false;
        }

        await _insumoRepository.AgregarAsync(insumo);
        await _insumoRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> ActualizarAsync(int id, Insumo insumo)
    {
        var insumoGuardado = await _insumoRepository.ObtenerPorIdAsync(id);

        if (insumoGuardado == null || insumo.CantidadDisponible < 0)
        {
            return false;
        }

        insumoGuardado.Nombre = insumo.Nombre;
        insumoGuardado.UnidadMedida = insumo.UnidadMedida;
        insumoGuardado.CantidadDisponible = insumo.CantidadDisponible;

        await _insumoRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var insumo = await _insumoRepository.ObtenerPorIdAsync(id);

        if (insumo == null)
        {
            return false;
        }

        await _insumoRepository.EliminarAsync(insumo);
        await _insumoRepository.GuardarCambiosAsync();

        return true;
    }
}