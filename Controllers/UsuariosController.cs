using CacaoTrigo.Models;
using CacaoTrigo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CacaoTrigo.Controllers;

/// <summary>Gestión de usuarios: solo para administradores.</summary>
[Authorize(Roles = RolesApp.Administrador)]
public class UsuariosController : Controller
{
    private readonly UserManager<ApplicationUser> _usuarios;

    public UsuariosController(UserManager<ApplicationUser> usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<IActionResult> Index()
    {
        var todos = await _usuarios.Users
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync();

        var lista = new List<UsuarioListadoViewModel>();

        foreach (var usuario in todos)
        {
            var roles = await _usuarios.GetRolesAsync(usuario);

            lista.Add(new UsuarioListadoViewModel
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Correo = usuario.Email ?? usuario.UserName ?? string.Empty,
                Rol = roles.Contains(RolesApp.Administrador)
                    ? RolesApp.Administrador
                    : RolesApp.Cliente
            });
        }

        return View(lista);
    }

    [HttpGet]
    public IActionResult CrearAdministrador()
    {
        return View(new CrearAdministradorViewModel());
    }

    // Desde aquí solo se pueden crear usuarios con rol Administrador.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearAdministrador(CrearAdministradorViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var usuario = new ApplicationUser
        {
            UserName = modelo.Correo.Trim(),
            Email = modelo.Correo.Trim(),
            EmailConfirmed = true,
            NombreCompleto = modelo.NombreCompleto.Trim()
        };

        var resultado = await _usuarios.CreateAsync(usuario, modelo.Contrasena);

        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(modelo);
        }

        await _usuarios.AddToRoleAsync(usuario, RolesApp.Administrador);

        TempData["Mensaje"] = $"Se creó el administrador {usuario.NombreCompleto}.";
        return RedirectToAction(nameof(Index));
    }
}
