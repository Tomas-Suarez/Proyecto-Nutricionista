using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<UsuarioEntity> Usuarios { get; set; }
        public DbSet<NutricionistaEntity> Nutricionistas { get; set; }
        public DbSet<PacienteEntity> Pacientes { get; set; }
        public DbSet<DietaEntity> Dietas { get; set; }
        public DbSet<PesajeEntity> Pesajes { get; set; }
        public DbSet<DietaComidaEntity> DietaComidas { get; set; }
        public DbSet<ComidaEntity> Comidas { get; set; }
        public DbSet<CategoriaComidaEntity> CategoriaComidas { get; set; }
        public DbSet<CategoriaEntity> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ComidaEntity>()
                .HasMany(c => c.Categorias)
                .WithMany(c => c.Comidas)
                .UsingEntity<CategoriaComidaEntity>(
                    j => j.HasOne(cc => cc.Categoria)
                          .WithMany()
                          .HasForeignKey(cc => cc.Id_Categoria),
                    j => j.HasOne(cc => cc.Comida)
                          .WithMany()
                          .HasForeignKey(cc => cc.Id_Comida),
                    j =>
                    {
                        j.ToTable("Categoria_Comida");
                        j.HasKey(cc => cc.Id_Categoria_Comida);
                    });
        }
    }
}