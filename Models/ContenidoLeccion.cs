namespace SistemaDeCursos.Models
{
	public class ContenidoLeccion
	{
		public int Id { get; set; }

		public int LeccionId { get; set; }
		public Leccion Leccion { get; set; }

		// Tipo de contenido: "Texto", "Imagen", "Video", "Pdf", "Archivo"
		public string Tipo { get; set; }

		// Para guardar texto
		public string? Texto { get; set; }

		// Para guardar ruta/URL de archivos, imágenes, PDFs o videos
		public string? UrlArchivo { get; set; }
		public string? PublicId { get; set; }

		// Orden del contenido dentro de una lección
		public int Orden { get; set; }
	}
}
