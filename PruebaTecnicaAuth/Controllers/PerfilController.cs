using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaAuth.Models;

namespace PruebaTecnicaAuth.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly UserManager<Usuario> _userManager;

        public PerfilController(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return RedirectToAction("Login", "Account");

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> Editar()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return RedirectToAction("Login", "Account");

            var modelo = new PerfilViewModel
            {
                Nombres = usuario.Nombre,
                PrimerApellido = usuario.PrimerApellido,
                SegundoApellido = usuario.SegundoApellido,
                Email = usuario.Email ?? string.Empty,
                Telefono = usuario.PhoneNumber,
                FechaNacimiento = usuario.FechaNacimiento
            };

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(PerfilViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return RedirectToAction("Login", "Account");

            usuario.Nombre = modelo.Nombres;
            usuario.PrimerApellido = modelo.PrimerApellido;
            usuario.SegundoApellido = modelo.SegundoApellido;
            usuario.Email = modelo.Email;
            usuario.PhoneNumber = modelo.Telefono;
            usuario.FechaNacimiento = modelo.FechaNacimiento;

            var resultado = await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(modelo);
            }

            TempData["Mensaje"] = "Tus datos se actualizaron correctamente.";
            return RedirectToAction("Index");
        }
    }
}