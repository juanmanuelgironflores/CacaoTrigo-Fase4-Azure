using System.ComponentModel.DataAnnotations;

namespace CacaoTrigo.Models;

public class Promocion
{
    public int PromocionId { get; set; }

    [Required]
    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Range(typeof(decimal), "0.01", "999999.99")]
    public decimal ValorDescuento { get; set; }

    public bool EsPorcentaje { get; set; } = true;

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    [StringLength(300)]
    public string? Condiciones { get; set; }

    public bool Activa { get; set; } = true;
}