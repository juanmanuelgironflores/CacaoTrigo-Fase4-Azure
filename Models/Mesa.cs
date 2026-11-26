using System.ComponentModel.DataAnnotations;

namespace CacaoTrigo.Models;

public class Mesa
{
    public int MesaId { get; set; }

    [Range(1, 999)]
    public int Numero { get; set; }

    [Range(1, 20)]
    public int Capacidad { get; set; }

    [StringLength(100)]
    public string? Ubicacion { get; set; }

    public bool Accesible { get; set; }
}