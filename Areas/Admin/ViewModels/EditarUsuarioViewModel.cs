using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.Areas.Admin.ViewModels
{
    public class EditarUsuarioViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public string Nombre { get; set; }

		[Required(ErrorMessage = "El campo es obligatorio")]
		public string Apellido { get; set; }

		[Required(ErrorMessage = "El campo es obligatorio")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "El campo es obligatorio")]
		public string Email { get; set; }

		[Required(ErrorMessage = "El campo es obligatorio")]
		public string Pais { get; set; }
		[Required(ErrorMessage = "El campo es obligatorio")]
		public string Provincia { get; set; }
		[Required(ErrorMessage = "El campo es obligatorio")]
		public string Ciudad { get; set; }
    }
}
