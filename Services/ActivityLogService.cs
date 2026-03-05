using SistemaDeCursos.Data;
using SistemaDeCursos.Models;

namespace SistemaDeCursos.Services
{
    public class ActivityLogService
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Registrar(string usuarioId, string accion)
        {
            _context.ActivityLogs.Add(new ActivityLog
            {
                UsuarioId = usuarioId,
                Accion = accion,
                Fecha = DateTime.Now
            });

            _context.SaveChanges();
        }

        public IEnumerable<ActivityLog> ObtenerLogs(string usuarioId)
        {
            return _context.ActivityLogs
                           .Where(l => l.UsuarioId == usuarioId)
                           .OrderByDescending(l => l.Fecha)
                           .Take(20)
                           .ToList();
        }
    }
}
