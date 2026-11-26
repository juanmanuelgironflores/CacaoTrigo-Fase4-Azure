using CacaoTrigo.Models;
using CacaoTrigo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CacaoTrigo.Controllers;

public class CuentaController : Controller
{
    private readonly UserManager<ApplicationUser> _usuarios;
    private readonly SignInManager<ApplicationUser> _sesion;

    public CuentaController(
        UserManager<ApplicationUser> usuarios,
        SignInManager<ApplicationUser> sesion)
    {
        _usuarios = usuarios;
        _sesion = sesion;
    }

    // ---------- Iniciar sesión ----------

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var usuario = await _usuarios.FindByEmailAsync(modelo.Correo.Trim());

        if (usuario != null)
        {
            var resultado = await _sesion.PasswordSignInAsync(
                usuario,
                modelo.Contrasena,
                modelo.Recordarme,
                lockoutOnFailure: true);

            if (resultado.Succeeded)
            {
                return RedirigirDespuesDeLogin(returnUrl);
            }

            if (resultado.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty,
                    "Demasiados intentos fallidos. Espera unos minutos e inténtalo de nuevo.");
                return View(modelo);
            }
        }

        // Mismo mensaje exista o no el correo: no revela qué cuentas existen.
        ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
        return View(modelo);
    }

    // ---------- Usuario nuevo (siempre con rol Cliente) ----------

    [HttpGet]
    public IActionResult Registro()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegistroViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel modelo)
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
            NombreCompleto = modelo.NombreCompleto.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(modelo.Telefono)
                ? null
                : modelo.Telefono.Trim()
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

        // Quien se registra por su cuenta es siempre cliente.
        await _usuarios.AddToRoleAsync(usuario, RolesApp.Cliente);
        await _sesion.SignInAsync(usuario, isPersistent: false);

        TempData["Mensaje"] = $"¡Bienvenido a Cacao y Trigo, {usuario.NombreCompleto}!";
        return RedirectToAction("Index", "Home");
    }

    // ---------- Cerrar sesión ----------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _sesion.SignOutAsync();
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    // ---------- Cambiar contraseña ----------

    [Authorize]
    [HttpGet]
    public IActionResult CambiarContrasena()
    {
        return View(new CambiarContrasenaViewModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarContrasena(CambiarContrasenaViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var usuario = await _usuarios.GetUserAsync(User);

        if (usuario == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var resultado = await _usuarios.ChangePasswordAsync(
            usuario,
            modelo.ContrasenaActual,
            modelo.NuevaContrasena);

        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(modelo);
        }

        // Mantiene la sesión abierta con la nueva contraseña.
        await _sesion.RefreshSignInAsync(usuario);

        TempData["Mensaje"] = "Tu contraseña se cambió correctamente.";
        return RedirectToAction("Index", "Home");
    }

    // ---------- Acceso denegado ----------

    public IActionResult AccesoDenegado()
    {
        return View();
    }

    private IActionResult RedirigirDespuesDeLogin(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
