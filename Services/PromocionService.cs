using CacaoTrigo.Models;
using CacaoTrigo.Repositories;

namespace CacaoTrigo.Services;

public class PromocionService : IPromocionService
{
    private readonly IPromocionRepository _promocionRepository;

    public PromocionService(IPromocionRepository promocionRepository)
    {
        _promocionRepository = promocionRepository;
    }

    public Task<List<Promocion>> ObtenerTodosAsync()
    {
        return _promocionRepository.ObtenerTodosAsync();
    }

    public Task<Promocion?> ObtenerPorIdAsync(int id)
    {
        return _promocionRepository.ObtenerPorIdAsync(id);
    }

    public async Task<bool> CrearAsync(Promocion promocion)
    {
        if (!DatosValidos(promocion))
        {
            return false;
        }

        await _promocionRepository.AgregarAsync(promocion);
        await _promocionRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> ActualizarAsync(int id, Promocion promocion)
    {
        var guardada = await _promocionRepository.ObtenerPorIdAsync(id);

        if (guardada == null || !DatosValidos(promocion))
        {
            return false;
        }

        guardada.Nombre = promocion.Nombre;
        guardada.Descripcion = promocion.Descripcion;
        guardada.ValorDescuento = promocion.ValorDescuento;
        guardada.EsPorcentaje = promocion.EsPorcentaje;
        guardada.FechaInicio = promocion.FechaInicio;
        guardada.FechaFin = promocion.FechaFin;
        guardada.Condiciones = promocion.Condiciones;
        guardada.Activa = promocion.Activa;

        await _promocionRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var promocion = await _promocionRepository.ObtenerPorIdAsync(id);

        if (promocion == null)
        {
            return false;
        }

        await _promocionRepository.EliminarAsync(promocion);
        await _promocionRepository.GuardarCambiosAsync();

        return true;
    }

    private static bool DatosValidos(Promocion promocion)
    {
        if (promocion.ValorDescuento < 0 ||
            promocion.FechaFin <= promocion.FechaInicio)
        {
            return false;
        }

        return !promocion.EsPorcentaje || promocion.ValorDescuento <= 100;
    }
}