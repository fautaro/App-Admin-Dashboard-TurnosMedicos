using Admin_Dashboard.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Turno> Turno { get; set; }

        public DbSet<Profesional> Profesional { get; set; }
        public DbSet<Profesion> Profesion { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuraciones adicionales de las tablas, relaciones, etc.
            modelBuilder.Entity<Turno>()
                .HasKey(e => e.Turno_Id);
            modelBuilder.Entity<Profesional>()
                .HasKey(e => e.Profesional_Id);
            modelBuilder.Entity<Profesion>()
                .HasKey(e => e.Profesion_Id);
        }
    }
}