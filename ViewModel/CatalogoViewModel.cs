using PortalAcademico.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.ViewModels
{
    public class CatalogoViewModel : IValidatableObject
    {
        public string? BuscarNombre { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "No se aceptan créditos negativos.")]
        public int? MinCreditos { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "No se aceptan créditos negativos.")]
        public int? MaxCreditos { get; set; }

        public TimeSpan? HorarioInicio { get; set; }
        public TimeSpan? HorarioFin { get; set; }

        public IEnumerable<Curso> Cursos { get; set; } = new List<Curso>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HorarioInicio.HasValue && HorarioFin.HasValue && HorarioInicio.Value >= HorarioFin.Value)
            {
                yield return new ValidationResult("El Horario de Fin no puede ser anterior al de Inicio.", new[] { nameof(HorarioFin) });
            }
        }
    }
}