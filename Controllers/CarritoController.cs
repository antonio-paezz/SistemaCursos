using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using SistemaDeCursos.Data;
using SistemaDeCursos.Models;
using SistemaDeCursos.ViewModels;
using System.Data;
using System.Security.Claims;
using SistemaDeCursos.Services;

namespace SistemaDeCursos.Controllers
{
	[Authorize]
	public class CarritoController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
		private readonly ActivityLogService _activityLogService;
        private readonly ILogger<CarritoController> _logger;

        public CarritoController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ActivityLogService activityLogService, ILogger<CarritoController> logger)
        {
            _userManager = userManager;
            _context = context;
            _activityLogService = activityLogService;
            _logger = logger;
        }

        public async Task<IActionResult> Carrito()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var items = await _context.CarritoProductos
                .Where(cp => cp.Carrito.UsuarioId == userId)
                .Select(cp => new CarritoViewModel
                {
                    CarritoProductoId = cp.Id,
                    Titulo = cp.curso.Titulo,
                    ImagenUrl = cp.curso.ImagenUrl,
                    Precio = cp.curso.Precio,
                    Cantidad = cp.Cantidad
                })
                .ToListAsync();

            var paginaViewModel = new CarritoPaginaViewModel
            {
                Items = items
            };

            return View(paginaViewModel);

        }
		
		public async Task<IActionResult> AgregarACarrito(int cursoId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Debe iniciar sesión." });

            var curso = await _context.Curso
                .FirstOrDefaultAsync(c => c.Id == cursoId);

            if (curso == null)
                return Json(new { success = false, message = "Curso no encontrado." });


            if (curso.InstructorId == userId)
            {
                return Json(new
                {
                    success = false,
                    message = "No podés agregar tu propio curso al carrito."
                });
            }



            var cursoDeUsuario = _context.Compra
                                                .Where(c => c.UsuarioId == userId)
                                                .Include(c => c.CompraDetalles)
                                                .ThenInclude(d => d.Curso)
                                                .SelectMany(c => c.CompraDetalles)
                                                .Any(cd => cd.Curso.Id == cursoId);

			if(cursoDeUsuario == true)
			{
                return Json(new { success = false, message = "El curso ya está comprado" });
            }

            var existe = _context.Carritos
												.Where(c => c.UsuarioId == userId)
												.Include(c => c.CarritoProductos)
												.ThenInclude(d => d.curso)
												.SelectMany(c => c.CarritoProductos)
												.Any(d => d.curso.Id == cursoId);

            if (existe)
                return Json(new { success = false, message = "El curso ya está en el carrito" });

            var carrito = _context.Carritos.FirstOrDefault(c => c.UsuarioId == userId);
            

            if (carrito == null)
            {
                carrito = new Carrito
                {
                    UsuarioId = userId,
					CarritoProductos = new List<CarritoProducto>()
                };

                await _context.Carritos.AddAsync(carrito);
                await _context.SaveChangesAsync();

            }

			var existente = await _context.CarritoProductos
			.FirstOrDefaultAsync(cp => cp.CarritoId == carrito.Id && cp.CursoId == cursoId);

			if (existente == null)
			{
				var carritoCurso = new CarritoProducto
				{
					CarritoId = carrito.Id,
					CursoId = cursoId,
					Cantidad = 1
				};

				_context.CarritoProductos.Add(carritoCurso);
			}
			else
			{
				existente.Cantidad++;
			}

			await _context.SaveChangesAsync();


            return Json(new { success = true, message = "El curso se agregó al carrito" });

        }

		public IActionResult Eliminar(int CarritoProductoId)
		{
            if (CarritoProductoId == 0)
            {
				TempData["ErrorMessage"] = "No se pudo eliminar el producto";
				return RedirectToAction("Carrito");
            }

			var carritoProducto = _context.CarritoProductos.Find(CarritoProductoId);

            try
            {
                _context.CarritoProductos.Remove(carritoProducto);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "El producto se eliminó con éxito";
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

            return RedirectToAction("Carrito");
		}


		public async Task<IActionResult> ConfirmarCompra(decimal total)
		{
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // 🚫 VALIDACIÓN EXTRA: evitar compra de cursos propios
                var tieneCursoPropioEnCarrito = await _context.CarritoProductos
                    .Where(cp => cp.Carrito.UsuarioId == userId)
                    .AnyAsync(cp => cp.curso.InstructorId == userId);

                if (tieneCursoPropioEnCarrito)
                {
                    TempData["ErrorMessage"] = "No podés comprar tu propio curso.";
                    return RedirectToAction("Carrito");
                }

                var usuarioIdParam = new SqlParameter("@usuarioId", userId);

                var resultadoParam = new SqlParameter("@resultado", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC ProcedimientoAlmacenadoParaCompra @usuarioId, @resultado OUTPUT",
                    usuarioIdParam,
                    resultadoParam
                );

                int resultado = (int)resultadoParam.Value;

                switch (resultado)
                {
                    case 1:
                        TempData["SuccessMessage"] = "La compra se realizó con éxito.";
                        _activityLogService.Registrar(userId, "Compró un curso");
                        break;

                    case -1:
                        TempData["ErrorMessage"] = "El usuario no se encontró.";
                        break;

                    case -2:
                        TempData["ErrorMessage"] = "El carrito está vacío.";
                        break;

                    case -3:
                        TempData["ErrorMessage"] = "El carrito contiene cursos propios.";
                        break;

                    default:
                        TempData["ErrorMessage"] = "Ocurrió un error inesperado en la compra.";
                        break;
                }
            }
            catch (SqlException ex)
            {
                //Error específico de SQL Server
                TempData["ErrorMessage"] = "Error en la base de datos al procesar la compra.";

                // Ideal: loggear el error
                _logger.LogError(ex, "Error SQL al confirmar compra para usuario {UserId}", userId);
            }
            catch (Exception ex)
            {
                // 🧨 Error general
                TempData["ErrorMessage"] = "Ocurrió un error inesperado.";

                _logger.LogError(ex, "Error general al confirmar compra para usuario {UserId}", userId);
            }

            return RedirectToAction("Carrito");
        }
	}
}
