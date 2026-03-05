 using Microsoft.AspNetCore.Mvc;
using SistemaDeCursos.Data;
using SistemaDeCursos.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using SistemaDeCursos.ViewModels;

namespace SistemaDeCursos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string? buscar, string? ordenar, int? categoriaId, decimal? precioMin, decimal? precioMax)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Cursos en carrito
            var cursosEnCarrito = _context.Carritos
                .Where(c => c.UsuarioId == userId)
                .SelectMany(c => c.CarritoProductos)
                .Select(d => d.curso.Id)
                .ToList();

            ViewBag.CursosEnCarrito = cursosEnCarrito;

            // Base query con calificaciones incluidas
            var query = _context.Curso
                .Include(c => c.Calificaciones)
                .Where(c => c.Estado == 1)
                .AsQueryable();

            // FILTRO DE BÚSQUEDA
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                query = query.Where(c =>
                    c.Titulo.Contains(buscar) ||
                    c.Descripcion.Contains(buscar));
            }

            // FILTRO POR CATEGORÍA
            if (categoriaId.HasValue)
            {
                query = query.Where(c => c.CategoriaId == categoriaId.Value);
            }

            // PRECIO MÍNIMO
            if (precioMin.HasValue)
            {
                query = query.Where(c => c.Precio >= precioMin.Value);
            }

            // PRECIO MÁXIMO
            if (precioMax.HasValue)
            {
                query = query.Where(c => c.Precio <= precioMax.Value);
            }


            // PROYECCIÓN
            var cursos = query
                .Select(c => new CursoViewModel
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Descripcion = c.Descripcion,
                    Precio = c.Precio,
                    FechaPublicacion = c.FechaPublicacion,
                    ImagenUrl = c.ImagenUrl,
                    Calificaciones = c.Calificaciones.ToList()
                })
                .ToList();

            // ORDENAMIENTO
            cursos = ordenar switch
            {
                "precioAsc" => cursos.OrderBy(c => c.Precio).ToList(),
                "precioDesc" => cursos.OrderByDescending(c => c.Precio).ToList(),
                "rating" => cursos.OrderByDescending(c => c.Promedio).ToList(),
                _ => cursos.OrderByDescending(c => c.FechaPublicacion).ToList()
            };

            return View(cursos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
