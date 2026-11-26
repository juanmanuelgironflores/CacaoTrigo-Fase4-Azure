using CacaoTrigo.Data;
using CacaoTrigo.Models;
using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly CacaoTrigoContext _context;

    public ReservaRepository(CacaoTrigoContext context)
    {
        _context = context;
    }

    public async Task<List<Reserva>> ObtenerTodosAsync()
    {
        return await _context.Reservas
            .Include(r => r.Mesa)
            .OrderByDescending(r => r.FechaHora)
            .ToListAsync();
    }

    public async Task<List<Reserva>> ObtenerPorUsuarioAsync(string usuarioId)
    {
        return await _context.Reservas
            .Include(r => r.Mesa)
            .Where(r => r.UsuarioId == usuarioId)
            .OrderByDescending(r => r.FechaHora)
            .ToListAsync();
    }

    public async Task<Reserva?> ObtenerPorIdAsync(int id)
    {
        return await _context.Reservas
            .Include(r => r.Mesa)
            .FirstOrDefaultAsync(r => r.ReservaId == id);
    }

    public Task<bool> MesaExisteAsync(int mesaId)
    {
        return _context.Mesas.AnyAsync(m => m.MesaId == mesaId);
    }

    public async Task AgregarAsync(Reserva reserva)
    {
        await _context.Reservas.AddAsync(reserva);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task EliminarAsync(Reserva reserva)
    {
        _context.Reservas.Remove(reserva);
        return Task.CompletedTask;
    }
}
