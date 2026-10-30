using Academia.Validacao;
using AcademiaApi.Dados;
using AcademiaApi.Modelos;
using Microsoft.EntityFrameworkCore;

namespace AcademiaApi.Services;

public class MensalidadeService(AcademiaContext db)
{
    // null = aluno não existe (vira 404 no controller)
    public async Task<List<MensalidadeDto>?> ListarDoAluno(int alunoId)
    {
        if (!await db.Alunos.AnyAsync(a => a.Id == alunoId)) return null;
        var lista = await db.Mensalidades.Where(m => m.AlunoId == alunoId).OrderBy(m => m.Vencimento).ToListAsync();
        return lista.Select(ParaDto).ToList();
    }

    public async Task<MensalidadeDto?> Obter(int id)
    {
        var m = await db.Mensalidades.FindAsync(id);
        return m is null ? null : ParaDto(m);
    }

    public async Task<MensalidadeDto> Criar(MensalidadeEntradaDto dto)
    {
        var erros = ValidadorMensalidade.Validar(dto.Valor, dto.Vencimento, null);
        if (!await db.Alunos.AnyAsync(a => a.Id == dto.AlunoId)) erros.Add("Aluno informado não existe.");
        if (erros.Count > 0) throw new RegraNegocioException(erros);

        var m = new Mensalidade { AlunoId = dto.AlunoId, Valor = dto.Valor, Vencimento = dto.Vencimento };
        db.Mensalidades.Add(m);
        await db.SaveChangesAsync();
        return ParaDto(m);
    }

    // Regra de negócio: a mesma mensalidade não pode ser paga duas vezes.
    public async Task<MensalidadeDto?> Pagar(int id)
    {
        var m = await db.Mensalidades.FindAsync(id);
        if (m is null) return null;
        if (m.DataPagamento is not null) throw new RegraNegocioException("Esta mensalidade já foi paga.");
        m.DataPagamento = DateOnly.FromDateTime(DateTime.Today);
        await db.SaveChangesAsync();
        return ParaDto(m);
    }

    // Regra de negócio: mensalidade paga é histórico financeiro e não pode ser excluída.
    public async Task<bool> Excluir(int id)
    {
        var m = await db.Mensalidades.FindAsync(id);
        if (m is null) return false;
        if (m.DataPagamento is not null) throw new RegraNegocioException("Não é possível excluir uma mensalidade já paga.");
        db.Mensalidades.Remove(m);
        await db.SaveChangesAsync();
        return true;
    }

    private static MensalidadeDto ParaDto(Mensalidade m)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var status = m.DataPagamento is not null ? "Paga" : m.Vencimento < hoje ? "Atrasada" : "Em aberto";
        return new MensalidadeDto(m.Id, m.AlunoId, m.Valor, m.Vencimento, m.DataPagamento, status);
    }
}
