using System.ComponentModel.DataAnnotations;

namespace CacaoTrigo.Models;

public class Producto
{
    public int ProductoId { get; set; }

    [Required]
    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Range(typeof(decimal), "0.01", "999999.99")]
    public decimal Precio { get; set; }

    [StringLength(300)]
    public string? ImagenUrl { get; set; }

    public bool Disponible { get; set; } = true;

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}