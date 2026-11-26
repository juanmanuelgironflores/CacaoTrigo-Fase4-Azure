using CacaoTrigo.Models;
using CacaoTrigo.Repositories;

namespace CacaoTrigo.Services;

public class MesaService : IMesaService
{
    private readonly IMesaRepository _mesaRepository;

    public MesaService(IMesaRepository mesaRepository)
    {
        _mesaRepository = mesaRepository;
    }

    public Task<List<Mesa>> ObtenerTodosAsync()
    {
        return _mesaRepository.ObtenerTodosAsync();
    }

    public Task<Mesa?> ObtenerPorIdAsync(int id)
    {
        return _mesaRepository.ObtenerPorIdAsync(id);
    }

    public async Task<bool> CrearAsync(Mesa mesa)
    {
        if (mesa.Numero <= 0 || mesa.Capacidad <= 0)
        {
            return false;
        }

        await _mesaRepository.AgregarAsync(mesa);
        await _mesaRepository.GuardarCambiosAsync();
        return true;
    }

    public async Task<bool> ActualizarAsync(int id, Mesa mesa)
    {
        var mesaGuardada = await _mesaRepository.ObtenerPorIdAsync(id);

        if (mesaGuardada == null || mesa.Numero <= 0 || mesa.Capacidad <= 0)
        {
            return false;
        }

        mesaGuardada.Numero = mesa.Numero;
        mesaGuardada.Capacidad = mesa.Capacidad;
        mesaGuardada.Ubicacion = mesa.Ubicacion;
        mesaGuardada.Accesible = mesa.Accesible;

        await _mesaRepository.GuardarCambiosAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var mesa = await _mesaRepository.ObtenerPorIdAsync(id);

        if (mesa == null)
        {
            return false;
        }

        await _mesaRepository.EliminarAsync(mesa);
        await _mesaRepository.GuardarCambiosAsync();
        return true;
    }
}