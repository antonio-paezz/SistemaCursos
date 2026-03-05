using SistemaDeCursos.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeCursos.ViewModels
{
	public class EditarCursoViewModelUsu
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Titulo { get; set; }

		[StringLength(500)]
		public string Descripcion { get; set; }

		public decimal Precio { get; set; }

		public string? ImagenUrl { get; set; }

		public string? PublicId { get; set; }

		public string? ResourceType { get; set; }

		public DateOnly FechaPublicacion { get; set; }

		// Categoria FK
		public int CategoriaId { get; set; }
		public IEnumerable<Categoria>? Categorias { get; set; }

		public IFormFile? Archivo { get; set; }
	}
}
