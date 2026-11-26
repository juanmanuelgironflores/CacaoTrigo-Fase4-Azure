using System.ComponentModel.DataAnnotations;

namespace CacaoTrigo.Models;

public class ProductoInsumo
{
    public int ProductoInsumoId { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    public int InsumoId { get; set; }
    public Insumo? Insumo { get; set; }

    [Range(typeof(decimal), "0.001", "999999.99")]
    public decimal CantidadNecesaria { get; set; }
}