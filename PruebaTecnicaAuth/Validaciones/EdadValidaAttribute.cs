using System.ComponentModel.DataAnnotations;

namespace PruebaTecnicaAuth.Validaciones
{
    public class EdadValidaAttribute : ValidationAttribute
    {
        private readonly int _edadMinima;
        private readonly int _edadMaxima;

        public EdadValidaAttribute(int edadMinima, int edadMaxima)
        {
            _edadMinima = edadMinima;
            _edadMaxima = edadMaxima;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Si no se cargó fecha de nacimiento, no es este atributo el que debe quejarse
            // (eso lo maneja [Required] si el campo es obligatorio)
            if (value is not DateTime fechaNacimiento)
            {
                return ValidationResult.Success;
            }

            var hoy = DateTime.Today;

            // 1. No puede ser una fecha futura
            if (fechaNacimiento.Date > hoy)
            {
                return new ValidationResult("La fecha de nacimiento no puede ser una fecha futura.");
            }

            // 2. Calcular edad exacta (considerando si ya cumplió años este año o no)
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad))
            {
                edad--;
            }

            // 3. Validar rango
            if (edad < _edadMinima)
            {
                return new ValidationResult($"Debes tener al menos {_edadMinima} años.");
            }

            if (edad > _edadMaxima)
            {
                return new ValidationResult($"La edad ingresada no es válida (máximo {_edadMaxima} años).");
            }

            return ValidationResult.Success;
        }
    }
}