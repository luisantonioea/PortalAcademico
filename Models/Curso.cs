using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models
{
    public class Curso : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public string Codigo { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Los créditos deben ser mayores a 0")]
        public int Creditos { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El cupo máximo debe ser mayor a 0")]
        public int CupoMaximo { get; set; }

        [Required]
        public TimeSpan HorarioInicio { get; set; }

        [Required]
        public TimeSpan HorarioFin { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Matricula> Matriculas { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HorarioInicio >= HorarioFin)
            {
                yield return new ValidationResult("El horario de inicio debe ser anterior al horario de fin.", new[] { nameof(HorarioInicio), nameof(HorarioFin) });
            }
        }
    }
}