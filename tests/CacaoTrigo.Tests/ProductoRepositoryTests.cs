using CacaoTrigo.Data;
using CacaoTrigo.Models;
using CacaoTrigo.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CacaoTrigo.Tests;

public sealed class ProductoRepositoryTests
{
    [Fact]
    public async Task ObtenerResumenVentasAsync_AgrupaPorProductoYSumaUnidadesEIngresos()
    {
        await using var contexto = CrearContexto();
        var categoria = new Categoria { CategoriaId = 1, Nombre = "Bebidas" };
        var productoMasVendido = new Producto
        {
            ProductoId = 1,
            Nombre = "Chocolate caliente",
            Precio = 5m,
            CategoriaId = categoria.CategoriaId,
            Categoria = categoria
        };
        var segundoProducto = new Producto
        {
            ProductoId = 2,
            Nombre = "Café",
            Precio = 2m,
            CategoriaId = categoria.CategoriaId,
            Categoria = categoria
        };
        var pedidoUno = new Pedido { PedidoId = 1, Estado = "Completado" };
        var pedidoDos = new Pedido { PedidoId = 2, Estado = "Completado" };

        contexto.AddRange(
            categoria,
            productoMasVendido,
            segundoProducto,
            pedidoUno,
            pedidoDos,
            new DetallePedido
            {
                PedidoId = pedidoUno.PedidoId,
                Pedido = pedidoUno,
                ProductoId = productoMasVendido.ProductoId,
                Producto = productoMasVendido,
                Cantidad = 2,
                PrecioUnitario = 5m,
                Subtotal = 10m
            },
            new DetallePedido
            {
                PedidoId = pedidoDos.PedidoId,
                Pedido = pedidoDos,
                ProductoId = productoMasVendido.ProductoId,
                Producto = productoMasVendido,
                Cantidad = 3,
                PrecioUnitario = 5m,
                Subtotal = 15m
            },
            new DetallePedido
            {
                PedidoId = pedidoUno.PedidoId,
                Pedido = pedidoUno,
                ProductoId = segundoProducto.ProductoId,
                Producto = segundoProducto,
                Cantidad = 4,
                PrecioUnitario = 2m,
                Subtotal = 8m
            });
        await contexto.SaveChangesAsync();

        var repositorio = new ProductoRepository(contexto);

        var resultado = await repositorio.ObtenerResumenVentasAsync();

        Assert.Collection(
            resultado,
            primero =>
            {
                Assert.Equal(productoMasVendido.ProductoId, primero.ProductoId);
                Assert.Equal("Chocolate caliente", primero.NombreProducto);
                Assert.Equal("Bebidas", primero.NombreCategoria);
                Assert.Equal(5, primero.UnidadesVendidas);
                Assert.Equal(25m, primero.Ingresos);
            },
            segundo =>
            {
                Assert.Equal(segundoProducto.ProductoId, segundo.ProductoId);
                Assert.Equal(4, segundo.UnidadesVendidas);
                Assert.Equal(8m, segundo.Ingresos);
            });
    }

    [Fact]
    public async Task ObtenerResumenVentasAsync_SinDetallesDevuelveListaVacia()
    {
        await using var contexto = CrearContexto();
        var repositorio = new ProductoRepository(contexto);

        var resultado = await repositorio.ObtenerResumenVentasAsync();

        Assert.Empty(resultado);
    }

    private static CacaoTrigoContext CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<CacaoTrigoContext>()
            .UseInMemoryDatabase($"CacaoTrigoTests-{Guid.NewGuid()}")
            .Options;

        return new CacaoTrigoContext(opciones);
    }
}
