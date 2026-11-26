using System.ComponentModel.DataAnnotations;

namespace CacaoTrigo.Models;

public class Pedido
{
    public int PedidoId { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required]
    [StringLength(30)]
    public string Estado { get; set; } = "Pendiente";

    [Range(typeof(decimal), "0.00", "999999.99")]
    public decimal Total { get; set; }

    [StringLength(120)]
    public string? NombreContacto { get; set; }

    [Phone]
    [StringLength(20)]
    public string? TelefonoContacto { get; set; }

    [StringLength(250)]
    public string? DireccionEntrega { get; set; }

    // Cliente que hizo el pedido (null si lo registró un administrador).
    public string? UsuarioId { get; set; }
    public ApplicationUser? Usuario { get; set; }
}