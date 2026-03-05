using SistemaDeCursos.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.Models
{
    public class DetalleCompra
    {
        public int Id { get; set; }
    
        public decimal PrecioUnitario { get; set; }

        // FK
        public int CompraId { get; set; }
        public Compra Compra { get; set; }

        // FK
        public int CursoId { get; set; }
        public Curso Curso { get; set; }
    }

}
