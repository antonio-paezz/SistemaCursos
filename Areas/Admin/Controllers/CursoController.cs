using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeCursos.Data;
using SistemaDeCursos.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Identity;
using SistemaDeCursos.Models;
using Microsoft.AspNetCore.Authorization;
using SistemaDeCursos.ViewModels;
using System.Security.Claims;

namespace SistemaDeCursos.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CursoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CloudinaryService _cloudinaryService;

        public CursoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, CloudinaryService cloudinaryService)
        {
            _context = context;
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Curso
                .Include(i => i.Instructor)
                .Include(i => i.Categoria)
                .ToListAsync();

            return View(cursos);
            
        }


        public async Task<IActionResult> VistaCrear()
        {
            var instructores = await _userManager.GetUsersInRoleAsync("Instructor");

            if (instructores == null)
            {
                TempData["ErrorMessage"] = "No se encontraron instructores";
                return RedirectToAction("Index");
            }

            var categorias = await _context.Categoria.ToListAsync();

            if (categorias == null)
            {
				TempData["ErrorMessage"] = "No se encontraron categorias";
				return RedirectToAction("Index");
			}

            var vm = new AgregarCursoViewModel
            {
                Instructores = instructores,
                Categorias = categorias
            };

            return View(vm);
        }

        public async Task<IActionResult> Crear(AgregarCursoViewModel vm) 
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessagge"] = "Datos incorrectos";
                return RedirectToAction("VistaCrear");
            }

            var result = new CloudinaryViewModel { };
            var curso = new Curso { };

            if(vm.Archivo != null)
            {
                result = await _cloudinaryService.SubirArchivoAsync(vm.Archivo);

                curso.Titulo = vm.Titulo;
                curso.Descripcion = vm.Descripcion;
                curso.Precio = vm.Precio;
                curso.Estado = vm.Estado;
                curso.ImagenUrl = result.UrlArchivo;
                curso.PublicId = result.PublicId;
                curso.ResourceType = result.Tipo;
                curso.FechaPublicacion = DateOnly.FromDateTime(DateTime.Now);
                curso.InstructorId = vm.InstructorId;
                curso.CategoriaId = vm.CategoriaId;
                
            }
            else
            {
				curso.Titulo = vm.Titulo;
				curso.Descripcion = vm.Descripcion;
				curso.Precio = vm.Precio;
				curso.Estado = vm.Estado;
				curso.FechaPublicacion = DateOnly.FromDateTime(DateTime.Now);
				curso.InstructorId = vm.InstructorId;
				curso.CategoriaId = vm.CategoriaId;
			}

            try
			{
				await _context.Curso.AddAsync(curso);
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

			TempData["SuccessMessagge"] = "Se creó el curso correctamente";
			return RedirectToAction("Index");
        }



        public async Task<IActionResult> VistaEditar(int id)
        {
			var instructores  = await _userManager.GetUsersInRoleAsync("Instructor");

			if (instructores == null)
			{
				TempData["SuccessMessagge"] = "No se encontraron instructores";
				return RedirectToAction("Index");
			}

			var categorias = _context.Categoria.ToList();

			if (categorias == null)
			{
				TempData["SuccessMessagge"] = "No se encontraron categorias";
				return RedirectToAction("Index");
			}

			var curso = _context.Curso
                .Select(c => new EditarCursoViewModel
            {
                Id = c.Id,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                Precio = c.Precio,
                Estado = c.Estado,
                ImagenUrl = c.ImagenUrl,
                Instructores = instructores,
                Categorias = categorias
            }).FirstOrDefault(c => c.Id == id);
            
            if (curso == null)
            {
                TempData["SuccessMessagge"] = "No se encontró el curso";
                return RedirectToAction("Index");
            }

			return View(curso);
        }

        public async Task<IActionResult> Editar(EditarCursoViewModel vm) 
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Los datos no son válidos");
                return View("VistaEditar",vm);
            }

            var curso = await _context.Curso.FindAsync( vm.Id );
            
            if (curso == null)
            {
                TempData["ErrorMessagge"] = "El curso no se encontró";
                return RedirectToAction("VistaEditar");
            }
            
            var result = new CloudinaryViewModel { };

            if (vm.Archivo != null)
            {
                result = await _cloudinaryService.SubirArchivoAsync(vm.Archivo);
                
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(curso.PublicId))
                    {
                        await _cloudinaryService.EliminarArchivo(curso.PublicId, curso.ResourceType);
                    }
                }

                curso.Id = vm.Id;
                curso.Titulo = vm.Titulo;
                curso.Descripcion = vm.Descripcion;
                curso.Precio = vm.Precio;
                curso.Estado = vm.Estado;
                curso.ImagenUrl = result.UrlArchivo;
                curso.PublicId = result.PublicId;
                curso.ResourceType = result.Tipo;
			    curso.FechaPublicacion = DateOnly.FromDateTime(DateTime.Now);
			    curso.InstructorId = vm.InstructorId;
                curso.CategoriaId = vm.CategoriaId;
            }
            else
            {
				curso.Id = vm.Id;
				curso.Titulo = vm.Titulo;
				curso.Descripcion = vm.Descripcion;
				curso.Precio = vm.Precio;
				curso.Estado = vm.Estado;
				curso.FechaPublicacion = DateOnly.FromDateTime(DateTime.Now);
				curso.InstructorId = vm.InstructorId;
				curso.CategoriaId = vm.CategoriaId;
			}

			try
			{
                _context.Curso.Update(curso);
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

            TempData["SuccessMessagge"] = "Se actualizó el curso correctamente";
			return RedirectToAction("Index");
        }


		public async Task<IActionResult> Eliminar(int id)
		{
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var tieneInscripciones = _context.Inscripcion
                .Any(i => i.CursoId == id);

            if (tieneInscripciones)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el curso porque tiene alumnos inscriptos.";
                return RedirectToAction("Index");
            }

            if (id == 0)
			{
				TempData["ErrorMessage"] = "Error al encontrar el curso";
				return RedirectToAction("Index");
			}

			var curso = _context.Curso.Find(id);

			if (curso == null)
			{
                TempData["ErrorMessage"] = "Error al encontrar el curso";
                return RedirectToAction("Index");
			}

			if (!string.IsNullOrEmpty(curso.PublicId))
			{
				await _cloudinaryService.EliminarArchivo(curso.PublicId, curso.ResourceType);
			}

			_context.Curso.Remove(curso);

			try
			{
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "El curso se eliminó correctamente";
			}
			catch (DbUpdateException ex)
			{
				// Error típico de base de datos: FK, UNIQUE, etc
				TempData["ErrorMessage"] = "Ocurrió un error al eliminar el curso";
				// Podés loguearlo
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Error inesperado. Intente más tarde";
			}


			return RedirectToAction("Index");
		}
	}
}
