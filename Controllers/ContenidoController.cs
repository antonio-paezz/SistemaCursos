using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeCursos.Data;
using SistemaDeCursos.Models;
using SistemaDeCursos.ViewModels;


namespace SistemaDeCursos.Controllers
{
	[Authorize(Roles = "Admin, Instructor")] 
	public class ContenidoController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly CloudinaryService _cloudinaryService;
        public ContenidoController(ApplicationDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
			_cloudinaryService = cloudinaryService;
        }

        public IActionResult VistaAgregar(int id)
		{
			var contenido = new ContenidoLeccionViewModel
			{
				LeccionId = id
			};
			return View(contenido);
		}

		[HttpPost]
		public async Task<IActionResult> AgregarContenido(ContenidoLeccionViewModel model)
		{
			if (!ModelState.IsValid)
				return View("VistaAgregar", model);

			var result = new CloudinaryViewModel { };
			var contenido = new ContenidoLeccion { };

            if (model.Archivo != null)
			{
				result = await _cloudinaryService.SubirArchivoAsync(model.Archivo);

				var nombreVisible = string.IsNullOrWhiteSpace(model.Texto)
					? Path.GetFileNameWithoutExtension(result.NombreArchivo)
					: model.Texto;
            
				contenido.LeccionId = model.LeccionId;
				contenido.Tipo = result.Tipo;
				contenido.Texto = nombreVisible;
				contenido.UrlArchivo = result.UrlArchivo;
				contenido.PublicId = result.PublicId;
            }
			else
			{

                contenido.LeccionId = model.LeccionId;
                contenido.Tipo = null;
                contenido.Texto = model.Texto;
                contenido.UrlArchivo = null;
                contenido.PublicId = null;
            }


			_context.Add(contenido);
			await _context.SaveChangesAsync();

			return RedirectToAction("Detalle", "Leccion" , new { id = model.LeccionId });
		}

		public async Task<IActionResult> EliminarContenido(int id)
		{
			var contenido = await _context.ContenidosLeccion.FindAsync(id);

			if (contenido == null)
				return NotFound();

			// Primero borro de Cloudinary
			if (!string.IsNullOrEmpty(contenido.PublicId))
			{
				await _cloudinaryService.EliminarArchivo(contenido.PublicId, contenido.Tipo);
			}

			// Luego borro de la BD
			_context.ContenidosLeccion.Remove(contenido);
			await _context.SaveChangesAsync();

			return RedirectToAction("Detalle", "Leccion",new { id = contenido.LeccionId });
		}


	}
}
