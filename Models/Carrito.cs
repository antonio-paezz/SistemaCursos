namespace SistemaDeCursos.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        
        public string UsuarioId { get; set; }

        public ICollection<CarritoProducto> CarritoProductos { get; set; }
    }
}
