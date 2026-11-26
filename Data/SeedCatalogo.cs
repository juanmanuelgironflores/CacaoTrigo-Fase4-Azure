using CacaoTrigo.Models;
using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Data;

public static class SeedCatalogo
{
    private sealed record ProductoInicial(
        string Nombre,
        string Categoria,
        string Descripcion,
        decimal Precio,
        string ImagenUrl);

    private static readonly ProductoInicial[] ProductosIniciales =
    [
        new("Chocolate caliente", "Bebidas", "Chocolate preparado con cacao y leche.", 2.75m, "/img/productos/chocolate-caliente.jpg"),
        new("Café de olla", "Bebidas", "Café aromático con canela.", 1.75m, "/img/productos/cafe-de-olla.jpg"),
        new("Café con leche", "Bebidas", "Café espresso acompañado de leche caliente.", 2.25m, "/img/productos/cafe-con-leche.jpg"),
        new("Semita de piña", "Pan dulce", "Pan dulce relleno de piña.", 1.25m, "/img/productos/semita-de-pina.jpg"),
        new("Quesadilla salvadoreña", "Pan dulce", "Pan horneado con queso y ajonjolí.", 1.50m, "/img/productos/quesadilla-salvadorena.jpg"),
        new("Pan de canela", "Pan dulce", "Pan suave con azúcar y canela.", 1.00m, "/img/productos/pan-de-canela.jpg"),
        new("Brownie de cacao", "Postres", "Brownie de chocolate con cacao.", 2.00m, "/img/productos/brownie-de-cacao.jpg"),
        new("Pastel de chocolate", "Postres", "Porción de pastel de chocolate.", 3.25m, "/img/productos/pastel-de-chocolate.jpg")
    ];

    public static async Task<int> InicializarAsync(
        CacaoTrigoContext contexto,
        CancellationToken cancellationToken = default)
    {
        var nombresCategoria = ProductosIniciales
            .Select(producto => producto.Categoria)
            .Distinct()
            .ToArray();

        foreach (var nombre in nombresCategoria)
        {
            var existe = await contexto.Categorias
                .AnyAsync(categoria => categoria.Nombre == nombre, cancellationToken);

            if (!existe)
            {
                await contexto.Categorias.AddAsync(
                    new Categoria { Nombre = nombre },
                    cancellationToken);
            }
        }

        await contexto.SaveChangesAsync(cancellationToken);

        var categorias = await contexto.Categorias
            .Where(categoria => nombresCategoria.Contains(categoria.Nombre))
            .ToDictionaryAsync(
                categoria => categoria.Nombre,
                categoria => categoria.CategoriaId,
                cancellationToken);

        var agregados = 0;
        var imagenesAsignadas = 0;

        foreach (var inicial in ProductosIniciales)
        {
            var existente = await contexto.Productos
                .FirstOrDefaultAsync(producto => producto.Nombre == inicial.Nombre, cancellationToken);

            if (existente != null)
            {
                if (string.IsNullOrWhiteSpace(existente.ImagenUrl))
                {
                    existente.ImagenUrl = inicial.ImagenUrl;
                    imagenesAsignadas++;
                }

                continue;
            }

            await contexto.Productos.AddAsync(
                new Producto
                {
                    Nombre = inicial.Nombre,
                    Descripcion = inicial.Descripcion,
                    Precio = inicial.Precio,
                    ImagenUrl = inicial.ImagenUrl,
                    Disponible = true,
                    CategoriaId = categorias[inicial.Categoria]
                },
                cancellationToken);

            agregados++;
        }

        if (agregados > 0 || imagenesAsignadas > 0)
        {
            await contexto.SaveChangesAsync(cancellationToken);
        }

        return agregados;
    }
}
