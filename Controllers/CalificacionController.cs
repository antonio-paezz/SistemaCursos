using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeCursos.Data;
using SistemaDeCursos.Models;
using SistemaDeCursos.ViewModels;
using System.Security.Claims;

namespace SistemaDeCursos.Controllers
{
    public class CalificacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalificacionController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost]
        public IActionResult Crear(AgregarCalificacionViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Datos inválidos" });
            }

            //Obtener usuario logueado
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity.Name;


            if (usuarioId == null)
            {
                return Unauthorized(new { success = false });
            }

            //Evitar que califique dos veces
            var yaExiste = _context.Calificacion
                .Any(c => c.CursoId == viewmodel.CursoId && c.UsuarioId == usuarioId);

            if (yaExiste)
            {
                return BadRequest(new { success = false, message = "Ya calificaste este curso" });
            }

            var calificacion = new Calificacion
            {
                Puntuacion = viewmodel.Puntuacion,
                Comentario = viewmodel.Comentario,
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                CursoId = viewmodel.CursoId,
                UsuarioId = usuarioId
            };

            try
            {
                _context.Calificacion.Add(calificacion);
                _context.SaveChanges();
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
            

            // Calcular nuevo promedio
            var promedio = _context.Calificacion
                .Where(c => c.CursoId == viewmodel.CursoId)
                .Average(c => c.Puntuacion);

            var total = _context.Calificacion
                .Count(c => c.CursoId == viewmodel.CursoId);

            return Json(new
            {
                success = true,
                promedio = Math.Round(promedio, 1),
                totalResenas = total,
                nuevaResena = new
                {
                    id = calificacion.Id,
                    usuario = userName,
                    fecha = calificacion.Fecha.ToString("dd/MM/yyyy"),
                    puntuacion = calificacion.Puntuacion,
                    comentario = calificacion.Comentario
                }
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Editar([FromBody]EditarCalificacionViewModel viewmodel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false });

            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var calificacion = _context.Calificacion
                .FirstOrDefault(c => c.Id == viewmodel.Id);

            if (calificacion == null)
                return NotFound(new { success = false });

            // solo el dueño puede editar
            if (calificacion.UsuarioId != usuarioId)
                return Unauthorized(new { success = false });

            calificacion.Puntuacion = viewmodel.Puntuacion;
            calificacion.Comentario = viewmodel.Comentario;

            _context.SaveChanges();

            // Recalcular promedio
            var promedio = _context.Calificacion
                .Where(c => c.CursoId == calificacion.CursoId)
                .Average(c => c.Puntuacion);

            var total = _context.Calificacion
                .Count(c => c.CursoId == calificacion.CursoId);

            return Json(new
            {
                success = true,
                promedio = Math.Round(promedio, 1),
                totalResenas = total,
                reseñaActualizada = new
                {
                    id = calificacion.Id,
                    puntuacion = calificacion.Puntuacion,
                    comentario = calificacion.Comentario
                }
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Eliminar([FromBody] EliminarCalificacionViewModel model)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var calificacion = _context.Calificacion
                .FirstOrDefault(c => c.Id == model.Id);

            if (calificacion == null)
                return NotFound(new { success = false });

            //solo el dueño puede eliminar
            if (calificacion.UsuarioId != usuarioId)
                return Unauthorized(new { success = false });

            var cursoId = calificacion.CursoId;

            _context.Calificacion.Remove(calificacion);
            _context.SaveChanges();

            var total = _context.Calificacion
                .Count(c => c.CursoId == cursoId);

            double promedio = 0;

            if (total > 0)
            {
                promedio = _context.Calificacion
                    .Where(c => c.CursoId == cursoId)
                    .Average(c => c.Puntuacion);
            }

            return Json(new
            {
                success = true,
                promedio = Math.Round(promedio, 1),
                totalResenas = total
            });
        }
    }
}
