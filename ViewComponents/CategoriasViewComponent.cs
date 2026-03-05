using Microsoft.AspNetCore.Mvc;
using SistemaDeCursos.Data;

namespace SistemaDeCursos.ViewComponents
{
    public class CategoriasViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoriasViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var categorias = _context.Categoria
                .OrderBy(c => c.Nombre)
                .ToList();

            return View(categorias);

        }
    }
}