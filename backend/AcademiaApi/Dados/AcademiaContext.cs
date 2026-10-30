using AcademiaApi.Modelos;
using Microsoft.EntityFrameworkCore;

namespace AcademiaApi.Dados;

public class AcademiaContext(DbContextOptions<AcademiaContext> options) : DbContext(options)
{
    public DbSet<Plano> Planos => Set<Plano>();
    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Mensalidade> Mensalidades => Set<Mensalidade>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Plano 1:N Aluno — não pode apagar plano que ainda tem alunos
        modelBuilder.Entity<Aluno>()
            .HasOne(a => a.Plano).WithMany(p => p.Alunos)
            .HasForeignKey(a => a.PlanoId).OnDelete(DeleteBehavior.Restrict);

        // Aluno 1:N Mensalidade — apagar o aluno apaga as mensalidades dele
        modelBuilder.Entity<Mensalidade>()
            .HasOne(m => m.Aluno).WithMany(a => a.Mensalidades)
            .HasForeignKey(m => m.AlunoId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Aluno>().HasIndex(a => a.Cpf).IsUnique();
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Login).IsUnique();
    }
}
