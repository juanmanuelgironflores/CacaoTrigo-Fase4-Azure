using CacaoTrigo.Data;
using CacaoTrigo.Models;
using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly CacaoTrigoContext _context;

    public ProductoRepository(CacaoTrigoContext context)
    {
        _context = context;
    }

    public async Task<List<Producto>> ObtenerTodosAsync()
    {
        return await _context.Productos
            .Include(producto => producto.Categoria)
            .OrderBy(producto => producto.Nombre)
            .ToListAsync();
    }

    public async Task<List<Producto>> BuscarYOrdenarAsync(
        string? buscar,
        int? categoriaId,
        string? ordenar)
    {
        IQueryable<Producto> consulta = _context.Productos
            .Include(producto => producto.Categoria);

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            buscar = buscar.Trim();

            consulta = consulta.Where(producto =>
                producto.Nombre.Contains(buscar) ||
                (producto.Descripcion != null &&
                 producto.Descripcion.Contains(buscar)));
        }

        if (categoriaId.HasValue)
        {
            consulta = consulta.Where(producto =>
                producto.CategoriaId == categoriaId.Value);
        }

        consulta = ordenar switch
        {
            "nombre_desc" => consulta.OrderByDescending(producto => producto.Nombre),
            "precio_asc" => consulta.OrderBy(producto => producto.Precio),
            "precio_desc" => consulta.OrderByDescending(producto => producto.Precio),
            _ => consulta.OrderBy(producto => producto.Nombre)
        };

        return await consulta.ToListAsync();
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id)
    {
        return await _context.Productos
            .Include(producto => producto.Categoria)
            .FirstOrDefaultAsync(producto => producto.ProductoId == id);
    }

    public async Task<bool> CategoriaExisteAsync(int categoriaId)
    {
        return await _context.Categorias
            .AnyAsync(categoria => categoria.CategoriaId == categoriaId);
    }

    public async Task AgregarAsync(Producto producto)
    {
        await _context.Productos.AddAsync(producto);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public Task EliminarAsync(Producto producto)
    {
        _context.Productos.Remove(producto);
        return Task.CompletedTask;
    }
}