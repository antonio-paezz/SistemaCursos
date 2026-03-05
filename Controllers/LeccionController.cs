using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SistemaDeCursos.Data;
using SistemaDeCursos.Models;
using SistemaDeCursos.ViewModels;

namespace SistemaDeCursos.Controllers
{
	public class LeccionController : Controller
	{
		private readonly ApplicationDbContext _context;
        private readonly CloudinaryService _cloudinaryService;
        public LeccionController(ApplicationDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public IActionResult VistaCrear(int id)
		{
			var leccion = new Leccion { CursoId = id };
			return View(leccion);
		}

		public async Task<IActionResult> Crear(Leccion leccion)
		{
			await _context.Leccion.AddAsync(leccion);

			try
			{
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "La lección se guardo correctamente";
				return RedirectToAction("Detalle", "Curso", new {id = leccion.CursoId});
			}
			catch (DbUpdateException ex)
			{
				// Error típico de base de datos: FK, UNIQUE, etc
				TempData["ErrorMessage"] = "Ocurrió un error al guardar la lección";
				// Podés loguearlo
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Error inesperado. Intente más tarde";
			}

			return View("VistaCrear", leccion); 
		}

		public IActionResult Detalle(int id) 
		{
			var leccion = _context.Leccion.Find(id);
			leccion.Contenidos = _context.ContenidosLeccion.Where(c => c.LeccionId == leccion.Id).ToList();
			return View(leccion); 
		}

        public IActionResult VerLeccion(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var leccion = _context.Leccion.Find(id);
            var contenidos = _context.ContenidosLeccion.Where(cl => cl.LeccionId == leccion.Id).ToList();

            leccion.Contenidos = contenidos;

            if (leccion == null)
            {
                return NotFound();
            }

            return View(leccion);
        }

        public async Task<IActionResult> Editar(int id) 
        {
            var leccion = await _context.Leccion.FindAsync(id);
            
            if (leccion == null)
            {
                TempData["ErrorMessagge"] = "No se econtró la lección";
                return RedirectToAction("Detalle", new {id = id});
            }
            var leccionvm = new EditarLeccionViewModel
            {
                Id = leccion.Id,
                Titulo = leccion.Titulo,
                Descripcion = leccion.Descripcion
            };

            return View(leccionvm); 
        }
        [HttpPost]
        public async Task<IActionResult> Editar(EditarLeccionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessagge"] = "Error al ingresar los datos";
                return View(vm);
            }

            var leccion = await _context.Leccion.FindAsync(vm.Id);
            if (leccion == null)
            {
				TempData["ErrorMessagge"] = "No se encontró la lección";
				return View(vm);
			}

            leccion.Titulo = vm.Titulo;
            leccion.Descripcion = vm.Descripcion;

            try
            {
                await _context.SaveChangesAsync();
                TempData["ErrorMessagge"] = "Se editó la lección correctamente";
                return RedirectToAction("Detalle", new {id = leccion.Id});
            }
            catch (DbUpdateException ex)
            {
                // Error de base de datos (FK, restricciones, etc.)
                ModelState.AddModelError("", "Error al guardar los cambios en la base de datos.");
            }
            catch (Exception ex)
            {
                // Error inesperado
                ModelState.AddModelError("", "Ocurrió un error inesperado.");
            }

            return View(vm);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var leccion = _context.Leccion
        .Include(l => l.Contenidos)
        .FirstOrDefault(l => l.Id == id);

            if (leccion == null)
            {
                TempData["ErrorMessage"] = "No se encontró la lección";
                return RedirectToAction("Lista");
            }

            // Eliminar archivos en Cloudinary
            foreach (var c in leccion.Contenidos)
            {
                await _cloudinaryService.EliminarArchivo(c.PublicId, c.Tipo);
            }

            try
            {
                _context.Leccion.Remove(leccion);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "La lección se eliminó correctamente";

                
                return RedirectToAction("Detalle", "Curso", new { id = leccion.CursoId });
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "No se pudo eliminar. Puede estar en uso.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error inesperado. Intente más tarde.";
            }

            return RedirectToAction("Detalle", "Curso", new { id = leccion.CursoId });
        }
    }
}
