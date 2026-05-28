using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.ViewModels
{
	public class RestablecerContraseniaViewModel
	{
		[Required]
		public string Email { get; set; }
		[Required(ErrorMessage = "El campo es obligatorio")]
		public string NuevaContrasenia { get; set; }
		[Required]
		public string ConfirmarContrasenia { get; set; }
	}
}
