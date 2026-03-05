using SistemaDeCursos.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.ViewModels
{
    public class LeccionViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        // FK curso
        public int CursoId { get; set; }
        public Curso Curso { get; set; }

        // Relación 1:N
        public ICollection<ContenidoLeccion> Contenidos { get; set; }
    }
}
