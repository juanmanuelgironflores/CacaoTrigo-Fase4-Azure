using CacaoTrigo.Models;
using CacaoTrigo.Repositories;

namespace CacaoTrigo.Services;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;

    public ProductoService(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public Task<List<Producto>> ObtenerTodosAsync()
    {
        return _productoRepository.ObtenerTodosAsync();
    }

    public Task<List<Producto>> BuscarYOrdenarAsync(
        string? buscar,
        int? categoriaId,
        string? ordenar)
    {
        return _productoRepository.BuscarYOrdenarAsync(
            buscar,
            categoriaId,
            ordenar);
    }

    public Task<Producto?> ObtenerPorIdAsync(int id)
    {
        return _productoRepository.ObtenerPorIdAsync(id);
    }

    public async Task<bool> CrearAsync(Producto producto)
    {
        if (!await _productoRepository.CategoriaExisteAsync(producto.CategoriaId))
        {
            return false;
        }

        await _productoRepository.AgregarAsync(producto);
        await _productoRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> ActualizarAsync(int id, Producto producto)
    {
        var productoGuardado =
            await _productoRepository.ObtenerPorIdAsync(id);

        if (productoGuardado == null ||
            !await _productoRepository.CategoriaExisteAsync(producto.CategoriaId))
        {
            return false;
        }

        productoGuardado.Nombre = producto.Nombre;
        productoGuardado.Descripcion = producto.Descripcion;
        productoGuardado.Precio = producto.Precio;
        productoGuardado.ImagenUrl = producto.ImagenUrl;
        productoGuardado.Disponible = producto.Disponible;
        productoGuardado.CategoriaId = producto.CategoriaId;

        await _productoRepository.GuardarCambiosAsync();

        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var producto = await _productoRepository.ObtenerPorIdAsync(id);

        if (producto == null)
        {
            return false;
        }

        await _productoRepository.EliminarAsync(producto);
        await _productoRepository.GuardarCambiosAsync();

        return true;
    }
}