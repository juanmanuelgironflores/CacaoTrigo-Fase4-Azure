using CacaoTrigo.Data;
using CacaoTrigo.Models;
using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly CacaoTrigoContext _context;

    public CategoriaRepository(CacaoTrigoContext context)
    {
        _context = context;
    }

    public async Task<List<Categoria>> ObtenerTodasAsync()
    {
        return await _context.Categorias
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<Categoria?> ObtenerPorIdAsync(int id)
    {
        return await _context.Categorias
            .FirstOrDefaultAsync(c => c.CategoriaId == id);
    }

    public async Task AgregarAsync(Categoria categoria)
    {
        await _context.Categorias.AddAsync(categoria);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task EliminarAsync(Categoria categoria)
    {
        _context.Categorias.Remove(categoria);
        return Task.CompletedTask;
    }
}