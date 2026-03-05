using Microsoft.AspNetCore.Identity;
using SistemaDeCursos.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Cursos dictados por el instructor
        public ICollection<Curso> CursosDictados { get; set; }

        // Cursos donde se inscribió como alumno
        public ICollection<Inscripcion> Inscripciones { get; set; }

        // Calificaciones hechas por el usuario
        public ICollection<Calificacion> Calificaciones { get; set; }

        // Compras realizadas
        public ICollection<Compra> Compras { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Pais { get; set; }
        public string Provincia { get; set; }
        public string Ciudad { get; set; }
        public string? FotoUrl { get; set; }
        public string? PublicId { get; set; }

    }

}
