using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestionEntradasInventario.Models;

namespace GestionEntradasInventario.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Entradas> Entradas { get; set; }
        public DbSet<EntradasDetalle> EntradasDetalle { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(
                new Producto
                {
                    ProductoId = 1,
                    Descripcion = "Huacal Rojo Mediano",
                    Costo = 300,
                    Precio = 450,
                    Existencia = 10
                },
                new Producto
                {
                    ProductoId = 2,
                    Descripcion = "Huacal Azul Grande",
                    Costo = 500,
                    Precio = 750,
                    Existencia = 5
                },
                new Producto
                {
                    ProductoId = 3,
                    Descripcion = "Caja Plástica Reforzada",
                    Costo = 150,
                    Precio = 250,
                    Existencia = 20
                }
            );
        }
    }
}