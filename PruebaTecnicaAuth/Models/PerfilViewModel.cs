using System.ComponentModel.DataAnnotations;

namespace PruebaTecnicaAuth.Models
{
    public class PerfilViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio.")]
        public string PrimerApellido { get; set; } = string.Empty;

        public string SegundoApellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Ingresa un teléfono válido.")]
        public string? Telefono { get; set; }

        public DateTime? FechaNacimiento { get; set; }
    }
}