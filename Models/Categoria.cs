using System.ComponentModel.DataAnnotations;

namespace CacaoTrigo.Models;

public class Categoria
{
    public int CategoriaId { get; set; }

    [Required]
    [StringLength(60)]
    public string Nombre { get; set; } = string.Empty;
}