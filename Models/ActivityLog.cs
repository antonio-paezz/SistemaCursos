namespace SistemaDeCursos.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }

        public string Accion { get; set; }

        public DateTime Fecha { get; set; }
    }
}
