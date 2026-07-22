using System.ComponentModel.DataAnnotations;
using PruebaTecnicaAuth.Validaciones;

namespace PruebaTecnicaAuth.Models
{
    public class PerfilViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio.")]
        public string PrimerApellido { get; set; } = string.Empty;

        public string SegundoApellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Ingresa un teléfono válido.")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        [EdadValida(16, 100, ErrorMessage = "La edad debe estar entre 16 y 100 años.")]
        public DateTime? FechaNacimiento { get; set; }
    }
}