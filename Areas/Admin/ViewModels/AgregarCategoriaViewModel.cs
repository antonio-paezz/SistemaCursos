using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.Areas.Admin.ViewModels
{
    public class AgregarCategoriaViewModel
    {
        [Required]
        [StringLength(80)]
        public string Nombre { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }
    }
}
