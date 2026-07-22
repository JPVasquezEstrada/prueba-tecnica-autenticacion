using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaAuth.Models;

namespace PruebaTecnicaAuth.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;

        public AccountController(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            // La validación de campos vacíos ahora la hacen las [Required] del ViewModel
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = await _userManager.FindByNameAsync(modelo.UserName);

            if (usuario == null)
            {
                ModelState.AddModelError("UserNameError", "Usuario incorrecto.");
                return View(modelo);
            }

            if (await _userManager.IsLockedOutAsync(usuario))
            {
                return RedirectToAction("Bloqueado", new { usuario = modelo.UserName });
            }

            var resultado = await _signInManager.PasswordSignInAsync(modelo.UserName, modelo.Password, isPersistent: false, lockoutOnFailure: true);

            if (resultado.IsLockedOut)
            {
                Console.WriteLine($"[EMAIL SIMULADO] Se notificó a {usuario.Email} que su cuenta fue bloqueada por 15 minutos.");
                return RedirectToAction("Bloqueado", new { usuario = modelo.UserName });
            }

            if (!resultado.Succeeded)
            {
                ModelState.AddModelError("PasswordError", "Contraseña incorrecta.");
                return View(modelo);
            }

            return RedirectToAction("Index", "Perfil");
        }

        [HttpGet]
        public async Task<IActionResult> Bloqueado(string usuario)
        {
            var usuarioEncontrado = await _userManager.FindByNameAsync(usuario);

            if (usuarioEncontrado?.LockoutEnd != null)
            {
                // LockoutEnd se guarda en UTC. Lo convertimos a hora de Perú (UTC-5) para mostrarlo bien.
                var zonaPeru = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                var horaLocalDesbloqueo = TimeZoneInfo.ConvertTime(usuarioEncontrado.LockoutEnd.Value, zonaPeru);

                var minutosRestantes = (usuarioEncontrado.LockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes;

                ViewBag.HoraDesbloqueo = horaLocalDesbloqueo.ToString("hh:mm tt");
                ViewBag.MinutosRestantes = Math.Max(0, (int)Math.Ceiling(minutosRestantes));
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> SesionExpirada()
        {
            await _signInManager.SignOutAsync();
            TempData["SesionExpirada"] = true;
            return RedirectToAction("Login");
        }
    }
}