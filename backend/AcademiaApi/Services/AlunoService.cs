using Academia.Validacao;
using AcademiaApi.Dados;
using AcademiaApi.Modelos;
using Microsoft.EntityFrameworkCore;

namespace AcademiaApi.Services;

public class AlunoService(AcademiaContext db)
{
    public const int DiasParaPrimeiroVencimento = 10;

    public async Task<List<AlunoDto>> Listar() =>
        await db.Alunos.OrderBy(a => a.Nome)
            .Select(a => new AlunoDto(a.Id, a.Nome, a.Cpf, a.Email, a.DataNascimento, a.PlanoId, a.Plano!.Nome, a.Telefone))
            .ToListAsync();

    public async Task<AlunoDto?> Obter(int id) =>
        await db.Alunos.Where(a => a.Id == id)
            .Select(a => new AlunoDto(a.Id, a.Nome, a.Cpf, a.Email, a.DataNascimento, a.PlanoId, a.Plano!.Nome, a.Telefone))
            .FirstOrDefaultAsync();

    // Regra de negócio: ao matricular, gera a primeira mensalidade com o valor do plano.
    public async Task<AlunoDto> Criar(AlunoEntradaDto dto)
    {
        var plano = await ValidarAsync(dto, idAtual: null);
        var aluno = new Aluno();
        Preencher(aluno, dto);
        aluno.Mensalidades.Add(new Mensalidade
        {
            Valor = plano.ValorMensal,
            Vencimento = DateOnly.FromDateTime(DateTime.Today).AddDays(DiasParaPrimeiroVencimento)
        });
        db.Alunos.Add(aluno);
        await db.SaveChangesAsync();
        return (await Obter(aluno.Id))!;
    }

    public async Task<bool> Atualizar(int id, AlunoEntradaDto dto)
    {
        var aluno = await db.Alunos.FindAsync(id);
        if (aluno is null) return false;
        await ValidarAsync(dto, idAtual: id);
        Preencher(aluno, dto);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Excluir(int id)
    {
        var aluno = await db.Alunos.FindAsync(id);
        if (aluno is null) return false;
        db.Alunos.Remove(aluno);
        await db.SaveChangesAsync();
        return true;
    }

    // Validação de campos vem da biblioteca; CPF único e plano existente são regras de negócio daqui.
    private async Task<Plano> ValidarAsync(AlunoEntradaDto dto, int? idAtual)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        // sem data, a idade não é checada pela biblioteca e entra a mensagem própria abaixo
        var erros = ValidadorAluno.Validar(dto.Nome, dto.Cpf, dto.Email, dto.DataNascimento ?? hoje.AddYears(-ValidadorAluno.IdadeMinima), hoje);
        if (dto.DataNascimento is null) erros.Add("Informe a data de nascimento.");
        erros.AddRange(ValidadorAluno.ValidarTelefone(dto.Telefone)); // regra nova da 1.1.0

        var cpf = SomenteDigitos(dto.Cpf);
        if (erros.Count == 0 && await db.Alunos.AnyAsync(a => a.Cpf == cpf && a.Id != idAtual))
            erros.Add("Já existe um aluno cadastrado com este CPF.");

        var plano = await db.Planos.FindAsync(dto.PlanoId);
        if (plano is null) erros.Add("Plano informado não existe.");

        if (erros.Count > 0) throw new RegraNegocioException(erros);
        return plano!;
    }

    private static void Preencher(Aluno aluno, AlunoEntradaDto dto)
    {
        aluno.Nome = dto.Nome!.Trim();
        aluno.Cpf = SomenteDigitos(dto.Cpf);
        aluno.Email = dto.Email!.Trim();
        aluno.DataNascimento = dto.DataNascimento!.Value;
        aluno.PlanoId = dto.PlanoId;
        aluno.Telefone = string.IsNullOrWhiteSpace(dto.Telefone) ? null : dto.Telefone.Trim();
    }

    private static string SomenteDigitos(string? texto) => new((texto ?? "").Where(char.IsDigit).ToArray());
}
