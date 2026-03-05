
namespace SistemaDeCursos.Models
{
    public class Inscripcion
{
    public int Id { get; set; }

    public DateOnly FechaInscripcion { get; set; }

    public int Estado { get; set; }

    // FK usuario
    public string UsuarioId { get; set; }
    public ApplicationUser Usuario { get; set; }

    // FK curso
    public int CursoId { get; set; }
    public Curso Curso { get; set; }
}

}
