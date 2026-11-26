using System.ComponentModel.DataAnnotations;

namespace CacaoTrigo.Models;

public class Insumo
{
    public int InsumoId { get; set; }

    [Required]
    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string UnidadMedida { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal CantidadDisponible { get; set; }
}