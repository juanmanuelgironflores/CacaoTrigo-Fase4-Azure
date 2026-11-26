using System.Linq;
using CacaoTrigo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CacaoTrigo.Data;

public static class SeedIdentity
{
    public static async Task InicializarAsync(
        IServiceProvider servicios,
        IConfiguration configuracion)
    {
        var roles = servicios.GetRequiredService<RoleManager<IdentityRole>>();
        var usuarios = servicios.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var rol in new[] { RolesApp.Administrador, RolesApp.Cliente })
        {
            if (!await roles.RoleExistsAsync(rol))
            {
                var resultadoRol = await roles.CreateAsync(new IdentityRole(rol));

                if (!resultadoRol.Succeeded)
                {
                    throw new InvalidOperationException(
                        "No se pudo crear el rol: " +
                        string.Join(" ", resultadoRol.Errors.Select(error => error.Description)));
                }
            }
        }

        var correo = configuracion["AdministradorInicial:Correo"];
        var contrasena = configuracion["AdministradorInicial:Contrasena"];
        var nombre = configuracion["AdministradorInicial:Nombre"] ?? "Administrador";
        var restablecer = configuracion.GetValue<bool>(
            "AdministradorInicial:RestablecerContrasena");

        if (string.IsNullOrWhiteSpace(correo) ||
            string.IsNullOrWhiteSpace(contrasena))
        {
            return;
        }

        var admin = await usuarios.FindByEmailAsync(correo);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = correo,
                Email = correo,
                EmailConfirmed = true,
                NombreCompleto = nombre
            };

            var creado = await usuarios.CreateAsync(admin, contrasena);

            if (!creado.Succeeded)
            {
                throw new InvalidOperationException(
                    "No se pudo crear el administrador inicial: " +
                    string.Join(" ", creado.Errors.Select(error => error.Description)));
            }
        }
        else if (restablecer)
        {
            var token = await usuarios.GeneratePasswordResetTokenAsync(admin);
            var resultado = await usuarios.ResetPasswordAsync(admin, token, contrasena);

            if (!resultado.Succeeded)
            {
                throw new InvalidOperationException(
                    "No se pudo restablecer la contraseña: " +
                    string.Join(" ", resultado.Errors.Select(error => error.Description)));
            }
        }

        if (!await usuarios.IsInRoleAsync(admin, RolesApp.Administrador))
        {
            var agregado = await usuarios.AddToRoleAsync(admin, RolesApp.Administrador);

            if (!agregado.Succeeded)
            {
                throw new InvalidOperationException(
                    "No se pudo asignar el rol de administrador: " +
                    string.Join(" ", agregado.Errors.Select(error => error.Description)));
            }
        }
    }
}