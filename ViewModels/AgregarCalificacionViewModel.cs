using SistemaDeCursos.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.ViewModels
{
    public class AgregarCalificacionViewModel
    {
        [Required(ErrorMessage = "El campo es obligatorio (*)")]
        [Range(1, 5)]
        public int Puntuacion { get; set; }

        [StringLength(150)]
        public string? Comentario { get; set; }
        public int CursoId { get; set; }
        
    }
}
