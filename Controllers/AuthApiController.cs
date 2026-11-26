using CacaoTrigo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthApiController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    // POST: api/auth/login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginApiRequest solicitud)
    {
        var usuario = await _userManager.FindByEmailAsync(solicitud.Correo);

        if (usuario == null)
        {
            return Unauthorized(new
            {
                mensaje = "Correo o contraseña incorrectos."
            });
        }

        var resultado = await _signInManager.CheckPasswordSignInAsync(
            usuario,
            solicitud.Contrasena,
            lockoutOnFailure: true);

        if (!resultado.Succeeded)
        {
            return Unauthorized(new
            {
                mensaje = "Correo o contraseña incorrectos."
            });
        }

        var roles = await _userManager.GetRolesAsync(usuario);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id),
            new(JwtRegisteredClaimNames.Email, usuario.Email ?? solicitud.Correo),
            new(ClaimTypes.NameIdentifier, usuario.Id),
            new(ClaimTypes.Name, usuario.UserName ?? solicitud.Correo),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(
            roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credenciales = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var vence = DateTime.UtcNow.AddMinutes(30);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: vence,
            signingCredentials: credenciales);

        return Ok(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(token),
            tokenType = "Bearer",
            expiresAt = vence
        });
    }
}

public sealed class LoginApiRequest
{
    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string Contrasena { get; set; } = string.Empty;
}