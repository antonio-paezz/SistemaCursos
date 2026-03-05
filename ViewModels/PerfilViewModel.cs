using SistemaDeCursos.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.ViewModels
{
    public class PerfilViewModel
    {
        public string? Nombre { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? FotoUrl { get; set; }
        public IEnumerable<ActivityLog>? Logs { get; set; }

        public IFormFile? Foto { get; set; }


        // cambio de contraseña
        [DataType(DataType.Password)]
        public string? ContraseñaActual { get; set; }
        [DataType(DataType.Password)]
        public string? NuevaContraseña { get; set; }
        [Compare("NuevaContraseña")]
        public string? ConfirmarContraseña { get; set; }

    }
}
