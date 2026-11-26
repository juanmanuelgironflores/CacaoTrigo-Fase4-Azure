using System.Security.Claims;
using CacaoTrigo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CacaoTrigo.Services;

/// <summary>Agrega el nombre completo del usuario a su sesión, para mostrarlo sin consultar la BD.</summary>
public class AppClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public const string TipoNombre = "nombre_completo";

    public AppClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> opciones)
        : base(userManager, roleManager, opciones)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identidad = await base.GenerateClaimsAsync(user);
        identidad.AddClaim(new Claim(TipoNombre, user.NombreCompleto ?? string.Empty));
        return identidad;
    }
}
