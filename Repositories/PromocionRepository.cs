using CacaoTrigo.Data;
using CacaoTrigo.Models;
using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Repositories;

public class PromocionRepository : IPromocionRepository
{
    private readonly CacaoTrigoContext _context;

    public PromocionRepository(CacaoTrigoContext context)
    {
        _context = context;
    }

    public async Task<List<Promocion>> ObtenerTodosAsync()
    {
        return await _context.Promociones
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<Promocion?> ObtenerPorIdAsync(int id)
    {
        return await _context.Promociones
            .FirstOrDefaultAsync(p => p.PromocionId == id);
    }

    public async Task AgregarAsync(Promocion promocion)
    {
        await _context.Promociones.AddAsync(promocion);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task EliminarAsync(Promocion promocion)
    {
        _context.Promociones.Remove(promocion);
        return Task.CompletedTask;
    }
}