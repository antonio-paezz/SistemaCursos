
namespace SistemaDeCursos.Models
{
    public class CarritoProducto
    {
        public int Id { get; set; }

        public int CarritoId { get; set; }
        public Carrito Carrito { get; set; }

        public int CursoId { get; set; }
        public Curso curso { get; set; }

        public int Cantidad { get; set; }
    }
}
