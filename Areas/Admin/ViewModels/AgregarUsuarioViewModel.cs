using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.Areas.Admin.ViewModels
{
    public class AgregarUsuarioViewModel
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        [Compare("Contraseña")]
        [DataType(DataType.Password)]
        public string ConfirmarContraseña { get; set; }

        [Required]
        public string Pais { get; set; }
        
        [Required]
        public string Provincia { get; set; }
        
        [Required]
        public string Ciudad { get; set; }
    }
}
