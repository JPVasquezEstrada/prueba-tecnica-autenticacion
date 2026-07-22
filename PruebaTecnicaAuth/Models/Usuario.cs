using Microsoft.AspNetCore.Identity;

namespace PruebaTecnicaAuth.Models
{
    public class Usuario : IdentityUser
    {
        public string Nombre { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string SegundoApellido { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
    }
}
