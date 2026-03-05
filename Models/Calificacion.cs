using SistemaDeCursos.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.Models
{
    public class Calificacion
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Puntuacion { get; set; }

        [StringLength(150)]
        public string? Comentario { get; set; }

        public DateOnly Fecha { get; set; }

        // FK usuario (string por Identity)
        public string UsuarioId { get; set; }
        public ApplicationUser Usuario { get; set; }

        // FK curso
        public int CursoId { get; set; }
        public Curso Curso { get; set; }
    }

}
