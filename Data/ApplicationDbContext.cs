using Microsoft.EntityFrameworkCore;
using KovakWeb.Models;

namespace KovakWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Estas propiedades representan a tus tablas de SQL Server
        public DbSet<Negocio> Negocios { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Pedido> Pedidos { get; set; } = null!;
        public DbSet<ProductoTalle> ProductoTalles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}