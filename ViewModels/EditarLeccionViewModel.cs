using SistemaDeCursos.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.ViewModels
{
    public class EditarLeccionViewModel
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }
    }
}
