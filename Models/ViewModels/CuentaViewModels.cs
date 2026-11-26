using System.ComponentModel.DataAnnotations;

namespace CacaoTrigo.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Escribe tu correo electrónico.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escribe tu contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;

    [Display(Name = "Mantener la sesión iniciada")]
    public bool Recordarme { get; set; }
}

public class RegistroViewModel
{
    [Required(ErrorMessage = "Escribe tu nombre completo.")]
    [StringLength(120)]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escribe tu correo electrónico.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150)]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
    [StringLength(20)]
    [Display(Name = "Teléfono (opcional)")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "Escribe una contraseña.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma tu contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarContrasena { get; set; } = string.Empty;
}

public class CambiarContrasenaViewModel
{
    [Required(ErrorMessage = "Escribe tu contraseña actual.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña actual")]
    public string ContrasenaActual { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escribe la nueva contraseña.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NuevaContrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma la nueva contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NuevaContrasena), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar nueva contraseña")]
    public string ConfirmarContrasena { get; set; } = string.Empty;
}

public class CrearAdministradorViewModel
{
    [Required(ErrorMessage = "Escribe el nombre completo.")]
    [StringLength(120)]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escribe el correo electrónico.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150)]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escribe una contraseña.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma la contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarContrasena { get; set; } = string.Empty;
}

public class UsuarioListadoViewModel
{
    public string Id { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
