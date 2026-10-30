using AcademiaApi.Modelos;
using Microsoft.AspNetCore.Identity;

namespace AcademiaApi.Dados;

public static class SeedDados
{
    // Só insere se as tabelas estiverem vazias, então reiniciar a API não apaga nem duplica nada.
    public static void Executar(AcademiaContext db)
    {
        if (!db.Usuarios.Any())
        {
            var admin = new Usuario { Login = "admin" };
            admin.SenhaHash = new PasswordHasher<Usuario>().HashPassword(admin, "admin123");
            db.Usuarios.Add(admin);
        }

        if (!db.Planos.Any())
        {
            var hoje = DateOnly.FromDateTime(DateTime.Today);
            var mensal = new Plano { Nome = "Mensal", ValorMensal = 120m, DuracaoMeses = 1 };
            var trimestral = new Plano { Nome = "Trimestral", ValorMensal = 100m, DuracaoMeses = 3 };
            var anual = new Plano { Nome = "Anual", ValorMensal = 80m, DuracaoMeses = 12 };

            Aluno NovoAluno(string nome, string cpf, string email, DateOnly nasc, Plano plano, bool pagouAnterior, bool pagouAtual) => new()
            {
                Nome = nome, Cpf = cpf, Email = email, DataNascimento = nasc, Plano = plano,
                Mensalidades =
                [
                    new() { Valor = plano.ValorMensal, Vencimento = hoje.AddMonths(-1), DataPagamento = pagouAnterior ? hoje.AddMonths(-1) : null },
                    new() { Valor = plano.ValorMensal, Vencimento = hoje.AddDays(-3), DataPagamento = pagouAtual ? hoje.AddDays(-4) : null },
                    new() { Valor = plano.ValorMensal, Vencimento = hoje.AddMonths(1) }
                ]
            };

            db.Alunos.AddRange(
                NovoAluno("Ana Beatriz Costa", "52998224725", "ana@email.com", new(1998, 3, 12), mensal, true, true),
                NovoAluno("Bruno Henrique Alves", "11144477735", "bruno@email.com", new(1990, 7, 25), trimestral, true, false),
                NovoAluno("Carla Mendes", "12345678909", "carla@email.com", new(2001, 11, 2), anual, false, false),
                NovoAluno("Diego Ferreira", "93541134780", "diego@email.com", new(1985, 1, 30), anual, true, true));
        }

        db.SaveChanges();
    }
}
