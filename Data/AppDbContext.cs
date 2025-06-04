using Microsoft.EntityFrameworkCore;
using AcademiaAPI.Models;

namespace AcademiaAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Aula> Aulas { get; set; }
        public DbSet<Inscrito> Inscritos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura chave composta para Inscrito
            modelBuilder.Entity<Inscrito>()
                .HasKey(i => new { i.IdAluno, i.IdAula });

            // Relacionamento Inscrito ? Aluno
            modelBuilder.Entity<Inscrito>()
                .HasOne(i => i.Aluno)
                .WithMany(a => a.Inscricoes)
                .HasForeignKey(i => i.IdAluno);

            // Relacionamento Inscrito ? Aula
            modelBuilder.Entity<Inscrito>()
                .HasOne(i => i.Aula)
                .WithMany(a => a.Inscricoes)
                .HasForeignKey(i => i.IdAula);
        }
    }
}
