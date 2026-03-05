using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.Areas.Admin.ViewModels
{
    public class EditarCategoriaViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(80)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(200)]
        public string Descripcion { get; set; }
    }
}
