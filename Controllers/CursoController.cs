using Microsoft.AspNetCore.Mvc;
using SistemaDeCursos.Data;
using SistemaDeCursos.ViewModels;
using SistemaDeCursos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SistemaDeCursos.Areas.Admin.ViewModels;
using Microsoft.Data.SqlClient;

namespace SistemaDeCursos.Controllers
{
    [Authorize]
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


        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> Lista()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cursos = await _context.Curso
                .FromSqlRaw("EXEC ProcedimientoAlmacenadoListaCursos @usuarioId",
                    new SqlParameter("@usuarioId", userId))
                .AsNoTracking()
                .ToListAsync();

            var cursosViewModel = cursos.Select(c => new CursoViewModel
            {
                Id = c.Id,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                Precio = c.Precio,
                FechaPublicacion = c.FechaPublicacion,
                ImagenUrl = c.ImagenUrl
            }).ToList();

            return View(cursosViewModel);
        }


        [Authorize(Roles = "Admin, Instructor")]
        public IActionResult VistaCrear()
        {
            var categorias = _context.Categoria.ToList();
            var viewmodel = new AgregarCursoViewModel
            {
                Categorias = categorias
            };

            return View(viewmodel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> Crear(AgregarCursoViewModel viewmodel) 
        {
            if(!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Los datos no son correctos";
                return RedirectToAction("VistaCrear");
            }

            var usuarioLogueado = await _userManager.FindByNameAsync(User.Identity.Name);
            var curso = new Curso { };


			if (viewmodel.Archivo != null)
            {
                var result = await _cloudinaryService.SubirArchivoAsync(viewmodel.Archivo);

                curso.Titulo = viewmodel.Titulo;
                curso.Descripcion = viewmodel.Descripcion;
                curso.Precio = viewmodel.Precio;
                curso.Estado = 1;
                curso.ImagenUrl = result.UrlArchivo;
                curso.PublicId = result.PublicId;
				curso.ResourceType = result.Tipo;
				curso.FechaPublicacion = DateOnly.FromDateTime(DateTime.Now);
                curso.InstructorId = usuarioLogueado.Id;
                curso.CategoriaId = viewmodel.CategoriaId;

            }
            else
            {
				curso.Titulo = viewmodel.Titulo;
				curso.Descripcion = viewmodel.Descripcion;
				curso.Precio = viewmodel.Precio;
				curso.Estado = 1;
				curso.FechaPublicacion = DateOnly.FromDateTime(DateTime.Now);
				curso.InstructorId = usuarioLogueado.Id;
				curso.CategoriaId = viewmodel.CategoriaId;
			}



			try
			{
                await _context.Curso.AddAsync(curso);
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "Curso guardado correctamente.";
				return RedirectToAction("Lista");
			}
			catch (DbUpdateException ex)
			{
				// Error típico de base de datos: FK, UNIQUE, etc
				TempData["ErrorMessage"] = "Ocurrió un error al guardar los datos.";
				return RedirectToAction("VistaCrear");
				// Podés loguearlo
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Error inesperado. Intente más tarde.";
				return RedirectToAction("VistaCrear");
			}
        }


        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> VistaEditar(int Id)
        {
            if(Id == 0)
            {
                TempData["ErrorMessage"] = "No se encontró el curso";
                return RedirectToAction("Lista");
            }

            var categorias = _context.Categoria.ToList();

			if (categorias == null)
			{
				TempData["ErrorMessage"] = "No se encontraron categorias";
				return RedirectToAction("Lista");
			}

			var curso = await _context.Curso.FindAsync(Id);

			if (curso == null)
			{
				TempData["ErrorMessage"] = "No se encontró el curso";
				return RedirectToAction("Lista");
			}

			var cursovm = new EditarCursoViewModelUsu
            {
			    Titulo = curso.Titulo,
				Descripcion = curso.Descripcion,
				Precio = curso.Precio,
				FechaPublicacion = curso.FechaPublicacion,
                ImagenUrl = curso.ImagenUrl,
                Categorias = categorias
			};
            
            return View(cursovm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> Editar(EditarCursoViewModelUsu viewmodel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "No se pudo editar el curso");
                return View(viewmodel);
            }
            var curso = await _context.Curso.FindAsync(viewmodel.Id);

            if (viewmodel.Archivo != null)
            {
                var result = await _cloudinaryService.SubirArchivoAsync(viewmodel.Archivo);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(curso.PublicId))
                    {
                        await _cloudinaryService.EliminarArchivo(curso.PublicId, curso.ResourceType);
                    }
                }

                curso.Id = viewmodel.Id;
				curso.Titulo = viewmodel.Titulo;
				curso.Descripcion = viewmodel.Descripcion;
				curso.Precio = viewmodel.Precio;
				curso.ImagenUrl = result.UrlArchivo;
				curso.PublicId = result.PublicId;
				curso.ResourceType = result.Tipo;
				curso.FechaPublicacion = DateOnly.FromDateTime(DateTime.Now);
				curso.CategoriaId = viewmodel.CategoriaId;
            }
            else
            {
				curso.Id = viewmodel.Id;
				curso.Titulo = viewmodel.Titulo;
				curso.Descripcion = viewmodel.Descripcion;
				curso.Precio = viewmodel.Precio;
				curso.FechaPublicacion = DateOnly.FromDateTime(DateTime.Now);
				curso.CategoriaId = viewmodel.CategoriaId;
			}


			try
			{
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "Curso guardado correctamente.";
				return RedirectToAction("Lista");
			}
			catch (DbUpdateException ex)
			{
				// Error típico de base de datos: FK, UNIQUE, etc
				ModelState.AddModelError("", "Ocurrió un error al guardar los datos.");
				// Podés loguearlo
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "Error inesperado. Intente más tarde.");
			}


			return View("VistaEditar", viewmodel);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> Eliminar(int id) 
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tieneInscripciones = _context.Inscripcion
                .Any(i => i.CursoId == id);

            if (tieneInscripciones)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el curso porque tiene alumnos inscriptos.";
                return RedirectToAction("Lista");
            }
            
            var curso = await _context.Curso.FindAsync(id);

            if (curso == null)
            {
                TempData["ErrorMessage"] = "Error al encontrar el curso";
                return RedirectToAction("Lista");
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
			

            return RedirectToAction("Lista");
        }
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> Detalle(int id)
        {

            var curso = await _context.Curso
                .Include(c => c.Lecciones)
                .Include(c => c.Calificaciones)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null)
            {
                TempData["ErrorMessage"] = "No se encontró el curso";
                return RedirectToAction("Lista");
            }

            var cursoViewModel = new CursoViewModel
            {
                Id = curso.Id,
                Titulo = curso.Titulo,
                Descripcion = curso.Descripcion,
                Precio = curso.Precio,
                FechaPublicacion = curso.FechaPublicacion,
                ImagenUrl = curso.ImagenUrl,

                Lecciones = curso.Lecciones?.ToList() ?? new List<Leccion>(),
                Calificaciones = curso.Calificaciones?.ToList() ?? new List<Calificacion>()
            };

            return View(cursoViewModel);
        }

        public IActionResult CursosDeUsuario()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cursos = _context.Compra
                .Where(c => c.UsuarioId == userId)
                .SelectMany(c => c.CompraDetalles)
                .Select(d => d.Curso)
                .Distinct()
                .Select(c => new CursoViewModel
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Descripcion = c.Descripcion,
                    Precio = c.Precio,
                    FechaPublicacion = c.FechaPublicacion,
                    InstructorId = c.InstructorId,
                    ImagenUrl = c.ImagenUrl,

                    Calificaciones = c.Calificaciones.ToList()
                })
                .ToList();

            return View(cursos);

        }

        public async Task<IActionResult> VerCurso(int cursoId, int? leccionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return RedirectToAction("Login", "Login");

            var curso = await _context.Curso
                .Include(c => c.Lecciones)
                    .ThenInclude(l => l.Contenidos)
                .Include(c => c.Calificaciones)
                    .ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            if (curso == null)
            {
                TempData["ErrorMessage"] = "No se encontró el curso";
                return RedirectToAction("Index", "Home");
            }

            var esInstructor = curso.InstructorId == userId;
            var esAdmin = User.IsInRole("Admin");

            //VALIDACIÓN DE ESTADO
            if (curso.Estado != 1 && !esInstructor && !esAdmin)
            {
                TempData["ErrorMessage"] = "El curso no está disponible.";
                return RedirectToAction("Index", "Home");
            }

            //VALIDACIÓN DE INSCRIPCIÓN (solo si no es instructor ni admin)
            if (!esInstructor && !esAdmin)
            {
                var inscripto = await _context.Inscripcion
                    .AnyAsync(i =>
                        i.Estado == 1 &&
                        i.UsuarioId == userId &&
                        i.CursoId == cursoId);

                if (!inscripto)
                {
                    TempData["ErrorMessage"] = "No estás inscripto a este curso.";
                    return RedirectToAction("Index", "Home");
                }
            }

            var yaCalifico = await _context.Calificacion
                .AnyAsync(c => c.CursoId == curso.Id && c.UsuarioId == userId);

            ViewBag.YaCalifico = yaCalifico;

            var leccionActual = leccionId == null
                ? curso.Lecciones.FirstOrDefault()
                : curso.Lecciones.FirstOrDefault(l => l.Id == leccionId);

            var cursoVm = new CursoViewModel
            {
                Id = curso.Id,
                Titulo = curso.Titulo,
                Descripcion = curso.Descripcion,
                Precio = curso.Precio,
                FechaPublicacion = curso.FechaPublicacion,
                InstructorId = curso.InstructorId,
                Lecciones = curso.Lecciones.ToList(),
                LeccionActual = leccionActual,
                Calificaciones = curso.Calificaciones
            };

            return View(cursoVm);
        }


        public IActionResult CargarLeccion(int id)
        {
            var leccion = _context.Leccion
                .Include(l => l.Contenidos)
                .FirstOrDefault(l => l.Id == id);

            return PartialView("_LeccionPartial", leccion);
        }
    }
}
