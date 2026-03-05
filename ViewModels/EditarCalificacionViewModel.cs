using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.ViewModels
{
    public class EditarCalificacionViewModel
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Puntuacion { get; set; }

        [StringLength(150)]
        public string Comentario { get; set; }
    }
}
