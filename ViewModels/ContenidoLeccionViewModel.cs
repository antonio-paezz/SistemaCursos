using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.ViewModels
{
	public class ContenidoLeccionViewModel
	{
        public int LeccionId { get; set; }

        public string? Tipo { get; set; }

        public string? Texto { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio (*)")]
        public IFormFile? Archivo { get; set; }
    }
}
