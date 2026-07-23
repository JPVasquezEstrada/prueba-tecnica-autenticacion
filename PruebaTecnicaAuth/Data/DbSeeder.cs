using Microsoft.AspNetCore.Identity;
using PruebaTecnicaAuth.Models;

namespace PruebaTecnicaAuth.Data
{
    public class DbSeeder
    {

        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();

            // Si ya existe, no se crea de nuevo
            var usuarioExistente = await userManager.FindByNameAsync("admin");
            if (usuarioExistente != null)
            {
                return;
            }

            var nuevoUsuario = new Usuario
            {
                UserName = "admin",
                Email = "admin@enchufate.pe",
                EmailConfirmed = true,
                Nombre = "Hugo Jean",
                PrimerApellido = "Vasquez",
                SegundoApellido = "Estrada"
            };

            var resultado = await userManager.CreateAsync(nuevoUsuario, "Admin123");

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    Console.WriteLine($"Error creando usuario semilla: {error.Description}");
                }
            }
        }
    }
}
