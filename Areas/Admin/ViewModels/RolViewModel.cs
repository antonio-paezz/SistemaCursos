using Microsoft.AspNetCore.Identity;
using SistemaDeCursos.Models;
using System.ComponentModel.DataAnnotations;


namespace SistemaDeCursos.Areas.Admin.ViewModels
{
    public class RolViewModel
    {
        public string? IdRol { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public string Nombre { get; set; }


        public string? usuarioId { get; set; }
        public IList<ApplicationUser>? Usuarios { get; set; }
    }
}
