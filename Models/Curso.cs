using SistemaDeCursos.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.Models{
    public class Curso
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        public decimal Precio { get; set; }

        public int Estado { get; set; } = 1;  // 1 = Activo

        public string? ImagenUrl { get; set; }

        public string? PublicId { get; set; }

        public string? ResourceType { get; set; }

        public DateOnly FechaPublicacion { get; set; }

        // Instructor FK (string)
        public string InstructorId { get; set; }
        public ApplicationUser Instructor { get; set; }

        // Categoria FK
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        // Relaciones 1:N
        public ICollection<Leccion> Lecciones { get; set; }
        public ICollection<Inscripcion> Inscripciones { get; set; }
        public ICollection<Calificacion> Calificaciones { get; set; }
        public ICollection<DetalleCompra> CompraDetalles { get; set; }
    }

}
