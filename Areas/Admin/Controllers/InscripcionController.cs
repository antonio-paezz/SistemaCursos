using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeCursos.Data;
using SistemaDeCursos.Models;
using System.Data;

namespace SistemaDeCursos.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class InscripcionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InscripcionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task <IActionResult> Index()
        {
            var inscripciones = await _context.Inscripcion
                .Include(i=> i.Usuario)
                .Include(i=> i.Curso)
                .ToListAsync();

            return View(inscripciones);
        }

		[HttpPost]
		public async Task<IActionResult> CambiarEstado(int id, int estado)
		{
			var inscripcion = _context.Inscripcion.FirstOrDefault(i => i.Id == id);
			
			if (inscripcion == null)
			{
				return Json(new { success = false, message = "No se encontró la inscripción" });
			}


			inscripcion.Estado = estado;
			
			try
			{
				await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Se cambió el estado correctamente." });
            }
			catch (DbUpdateException ex)
			{
				// Error de base de datos (FK, restricciones, etc.)
				return Json(new { success = false, message = "Error al guardar los cambios." });
			}
			catch (Exception ex)
			{
				// Error inesperado
				ModelState.AddModelError("", "Ocurrió un error inesperado.");
			}

			return RedirectToAction("Index");
		}
	}
}
