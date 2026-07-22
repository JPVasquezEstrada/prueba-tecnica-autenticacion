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
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string userName, string password)
        {
            // 1. Validar campos vacíos
            if (string.IsNullOrWhiteSpace(userName))
            {
                ModelState.AddModelError("UserNameError", "El usuario es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("PasswordError", "La contraseña es obligatoria.");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            // 2. Buscar el usuario
            var usuario = await _userManager.FindByNameAsync(userName);

            if (usuario == null)
            {
                ModelState.AddModelError("UserNameError", "Usuario incorrecto.");
                return View();
            }

            // 3. Verificar si está bloqueado ANTES de validar contraseña
            if (await _userManager.IsLockedOutAsync(usuario))
            {
                return RedirectToAction("Bloqueado", new { usuario = userName });
            }

            // 4. Intentar iniciar sesión (esto internamente valida la contraseña
            //    y AUTOMÁTICAMENTE incrementa AccessFailedCount si falla,
            //    y bloquea la cuenta si llega al máximo, gracias a Identity)
            var resultado = await _signInManager.PasswordSignInAsync(userName, password, isPersistent: false, lockoutOnFailure: true);

            if (resultado.IsLockedOut)
            {
                Console.WriteLine($"[EMAIL SIMULADO] Se notificó a {usuario.Email} que su cuenta fue bloqueada por 15 minutos.");
                return RedirectToAction("Bloqueado", new { usuario = userName });
            }

            if (!resultado.Succeeded)
            {
                ModelState.AddModelError("PasswordError", "Contraseña incorrecta.");
                return View();
            }

            // 5. Éxito -> redirige al perfil
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