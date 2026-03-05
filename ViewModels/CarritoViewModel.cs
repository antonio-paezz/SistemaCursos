using SistemaDeCursos.Models;

namespace SistemaDeCursos.ViewModels
{
	public class CarritoViewModel
	{
		public int CarritoProductoId { get; set; }
		public string Titulo { get; set; }
        public string ImagenUrl { get; set; }
        public decimal Precio { get; set; }
		public int Cantidad { get; set; }

		public decimal Subtotal => Precio * Cantidad;

	}
}
