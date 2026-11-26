namespace CacaoTrigo.Models;

public sealed record ResumenVentasProducto(
    int ProductoId,
    string NombreProducto,
    string NombreCategoria,
    int UnidadesVendidas,
    decimal Ingresos);
