using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaDeCursos.Models;

namespace SistemaDeCursos.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }



        public DbSet<Calificacion> Calificacion { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Compra> Compra { get; set; }
        public DbSet<Curso> Curso { get; set; }
        public DbSet<DetalleCompra> DetalleCompra { get; set; }
        public DbSet<Inscripcion> Inscripcion { get; set; }
        public DbSet<Leccion> Leccion { get; set; }
		public DbSet<ContenidoLeccion> ContenidosLeccion { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoProducto> CarritoProductos { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
    }
}
