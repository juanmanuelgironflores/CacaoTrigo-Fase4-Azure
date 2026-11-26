using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CacaoTrigo.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(120)]
    public string NombreCompleto { get; set; } = string.Empty;
}