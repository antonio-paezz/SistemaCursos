using SistemaDeCursos.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.ViewModels
{
	public class CursoViewModel
	{
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; }

        [StringLength(300)]
        public string Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        public DateOnly FechaPublicacion { get; set; }

        public string InstructorId { get; set; }

        public string ImagenUrl { get; set; }

        public string ImagenMostrada =>
            string.IsNullOrEmpty(ImagenUrl)
                ? "/img/default-course.png"
                : ImagenUrl;

        // 🔹 Lecciones
        public int LeccionId { get; set; }
        public IEnumerable<Leccion> Lecciones { get; set; }
        public Leccion? LeccionActual { get; set; }

        // 🔹 Calificaciones
        public IEnumerable<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();

        // 🔹 Datos calculados (PRO)
        public double Promedio =>
            Calificaciones.Any()
                ? Math.Round(Calificaciones.Average(c => c.Puntuacion), 1)
                : 0;

        public int TotalResenas => Calificaciones.Count();
    }
}
