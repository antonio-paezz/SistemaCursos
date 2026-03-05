using Microsoft.AspNetCore.Identity;
using SistemaDeCursos.Models;

namespace SistemaDeCursos.Areas.Admin.ViewModels
{
    public class RolesDeUsuarioViewModel
    {
        public string UsuarioId { get; set; }
        public string RolName { get; set; }
        public List<string>? RolesDeUsuario { get; set; }
        public List<IdentityRole>? Roles { get; set; }
    }
}
