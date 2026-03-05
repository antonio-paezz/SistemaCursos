using SistemaDeCursos.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.Models
{
    public class Compra
    {
        public int Id { get; set; }

        public DateOnly FechaCompra { get; set; }

        public decimal Total { get; set; }

        // FK usuario
        public string UsuarioId { get; set; }
        public ApplicationUser Usuario { get; set; }

        public ICollection<DetalleCompra> CompraDetalles { get; set; }
    }

}
