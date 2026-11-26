using CacaoTrigo.Data;
using CacaoTrigo.Models;
using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Repositories;

public class InsumoRepository : IInsumoRepository
{
    private readonly CacaoTrigoContext _context;

    public InsumoRepository(CacaoTrigoContext context)
    {
        _context = context;
    }

    public async Task<List<Insumo>> ObtenerTodosAsync()
    {
        return await _context.Insumos
            .OrderBy(insumo => insumo.Nombre)
            .ToListAsync();
    }

    public async Task<Insumo?> ObtenerPorIdAsync(int id)
    {
        return await _context.Insumos
            .FirstOrDefaultAsync(insumo => insumo.InsumoId == id);
    }

    public async Task AgregarAsync(Insumo insumo)
    {
        await _context.Insumos.AddAsync(insumo);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task EliminarAsync(Insumo insumo)
    {
        _context.Insumos.Remove(insumo);
        return Task.CompletedTask;
    }
}