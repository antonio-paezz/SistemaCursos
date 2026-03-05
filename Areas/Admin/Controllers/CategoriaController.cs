using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeCursos.Areas.Admin.ViewModels;
using SistemaDeCursos.Data;
using SistemaDeCursos.Models;

namespace SistemaDeCursos.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class CategoriaController : Controller
	{
		private readonly ApplicationDbContext _context;
        public CategoriaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
		{
			var categorias = _context.Categoria.ToList();
			return View(categorias);
		}

		public IActionResult VistaCrear()
		{
			return View();
		}

		public async Task<IActionResult> Crear(AgregarCategoriaViewModel viewmodel)
		{
            if (!ModelState.IsValid)
            {
				TempData["ErrorMessagge"] = "Los datos no son válidos";
				return RedirectToAction("Index");
            }

			var categoria = new Categoria
			{
				Nombre = viewmodel.Nombre,
				Descripcion = viewmodel.Descripcion
			};

            try
            {
                await _context.Categoria.AddAsync(categoria);
                await _context.SaveChangesAsync();
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

            TempData["SuccessMessagge"] = "Se creó la categoría correctamente";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> VistaEditar(int id)
        {
            if (id == 0)
            {
                TempData["ErrorMessagge"] = "No se encontró la categoría";
                return RedirectToAction("Index");
            }

            var categoria = _context.Categoria
                .Where(c => c.Id == id)
                .Select(c => new EditarCategoriaViewModel
            {
                Id = id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion
            })
            .FirstOrDefault();

            if (categoria == null)
            {
                TempData["ErrorMessagge"] = "No se encontró la categoría";
                return RedirectToAction("Index");
            }

            return View(categoria);
        }

        public async Task<IActionResult> Editar(EditarCategoriaViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessagge"] = "Los datos no son válidos";
                return RedirectToAction("VistaEditar");
            }

            var categoria = new Categoria
            {
                Id = viewmodel.Id,
                Nombre = viewmodel.Nombre,
                Descripcion = viewmodel.Descripcion
            };

            try
            {
                _context.Categoria.Update(categoria);
                await _context.SaveChangesAsync();
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

            TempData["SuccessMessagge"] = "Se creó la categoría correctamente";
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Eliminar(int id)
        {
            if (id == 0)
            {
                TempData["ErrorMessagge"] = "No se encontró la categoría";
                return RedirectToAction("Index");
            }

            var categoria = await _context.Categoria.FindAsync(id);

            if (categoria == null)
            {
                TempData["ErrorMessagge"] = "No se encontró la categoría";
                return RedirectToAction("Index");
            }

            var tieneCursos = await _context.Curso
                .AnyAsync(c => c.CategoriaId == id);

            if (tieneCursos)
            {
                TempData["ErrorMessage"] =
                    "No se puede eliminar la categoría porque tiene cursos asociados.";
                return RedirectToAction("Index");
            }


            try
            {
                _context.Categoria.Remove(categoria);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "La categoría se eliminó correctamente.";
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

            TempData["SuccessMessagge"] = "La categoría se eliminó correctamente";
            return RedirectToAction("Index");
        }

    }
}
