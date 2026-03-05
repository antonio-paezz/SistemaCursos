using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeCursos.Data;

namespace SistemaDeCursos.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class DashboardController : Controller
	{
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
		{
            ViewBag.TotalCursos = _context.Curso.Count();
            ViewBag.TotalUsuarios = _context.Users.Count();
            ViewBag.TotalVentas = _context.Compra.Count();
            ViewBag.Ingresos = _context.Compra.Sum(c => c.Total);

            // Últimos cursos
            var ultimosCursos = _context.Curso
                .OrderByDescending(c => c.FechaPublicacion)
                .Take(5)
                .Select(c => new
                {
                    c.Titulo,
                    c.FechaPublicacion
                })
                .ToList();

            ViewBag.UltimosCursos = ultimosCursos;

            // Ventas por mes (para gráfico)
            var ventasMes = _context.Compra
                .GroupBy(c => new { c.FechaCompra.Year, c.FechaCompra.Month })
                .Select(g => new
                {
                    Mes = $"{g.Key.Month}/{g.Key.Year}",
                    Total = g.Count()
                })
                .ToList();

            ViewBag.VentasMes = ventasMes;

            return View();
        }
	}
}
