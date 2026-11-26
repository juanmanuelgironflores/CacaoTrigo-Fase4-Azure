using System.ComponentModel.DataAnnotations;

namespace CacaoTrigo.Models;

public class Reserva
{
    public int ReservaId { get; set; }

    [Display(Name = "Fecha y hora")]
    public DateTime FechaHora { get; set; }

    [Display(Name = "Número de personas")]
    [Range(1, 20)]
    public int NumeroPersonas { get; set; }

    [Display(Name = "Nombre de contacto")]
    [Required]
    [StringLength(120)]
    public string NombreContacto { get; set; } = string.Empty;

    [Display(Name = "Teléfono de contacto")]
    [Required]
    [Phone]
    [StringLength(20)]
    public string TelefonoContacto { get; set; } = string.Empty;

    [Display(Name = "Correo de contacto")]
    [EmailAddress]
    [StringLength(150)]
    public string? CorreoContacto { get; set; }

    [Display(Name = "Estado")]
    [Required]
    [StringLength(20)]
    public string Estado { get; set; } = "Pendiente";

    [Display(Name = "Zona")]
    [StringLength(20)]
    public string? Zona { get; set; }

    // La mesa la asigna el administrador al confirmar la reserva.
    [Display(Name = "ID de mesa")]
    public int? MesaId { get; set; }

    [Display(Name = "Mesa asignada")]
    public Mesa? Mesa { get; set; }

    // Cliente que hizo la reserva (null si la registró un administrador).
    public string? UsuarioId { get; set; }
    public ApplicationUser? Usuario { get; set; }
}