using CacaoTrigo.Data;
using CacaoTrigo.Models;
using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Repositories;

public class MesaRepository : IMesaRepository
{
    private readonly CacaoTrigoContext _context;

    public MesaRepository(CacaoTrigoContext context)
    {
        _context = context;
    }

    public async Task<List<Mesa>> ObtenerTodosAsync()
    {
        return await _context.Mesas
            .OrderBy(m => m.Numero)
            .ToListAsync();
    }

    public async Task<Mesa?> ObtenerPorIdAsync(int id)
    {
        return await _context.Mesas
            .FirstOrDefaultAsync(m => m.MesaId == id);
    }

    public async Task AgregarAsync(Mesa mesa)
    {
        await _context.Mesas.AddAsync(mesa);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task EliminarAsync(Mesa mesa)
    {
        _context.Mesas.Remove(mesa);
        return Task.CompletedTask;
    }
}