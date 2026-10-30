using Academia.Validacao;
using AcademiaApi.Dados;
using AcademiaApi.Modelos;
using Microsoft.EntityFrameworkCore;

namespace AcademiaApi.Services;

public class PlanoService(AcademiaContext db)
{
    public async Task<List<PlanoDto>> Listar() =>
        await db.Planos.OrderBy(p => p.Nome)
            .Select(p => new PlanoDto(p.Id, p.Nome, p.ValorMensal, p.DuracaoMeses, p.Alunos.Count))
            .ToListAsync();

    public async Task<PlanoDto?> Obter(int id) =>
        await db.Planos.Where(p => p.Id == id)
            .Select(p => new PlanoDto(p.Id, p.Nome, p.ValorMensal, p.DuracaoMeses, p.Alunos.Count))
            .FirstOrDefaultAsync();

    public async Task<PlanoDto> Criar(PlanoEntradaDto dto)
    {
        Validar(dto);
        var plano = new Plano { Nome = dto.Nome!.Trim(), ValorMensal = dto.ValorMensal, DuracaoMeses = dto.DuracaoMeses };
        db.Planos.Add(plano);
        await db.SaveChangesAsync();
        return new PlanoDto(plano.Id, plano.Nome, plano.ValorMensal, plano.DuracaoMeses, 0);
    }

    public async Task<bool> Atualizar(int id, PlanoEntradaDto dto)
    {
        var plano = await db.Planos.FindAsync(id);
        if (plano is null) return false;
        Validar(dto);
        plano.Nome = dto.Nome!.Trim();
        plano.ValorMensal = dto.ValorMensal;
        plano.DuracaoMeses = dto.DuracaoMeses;
        await db.SaveChangesAsync();
        return true;
    }

    // Regra de negócio: plano com alunos matriculados não pode ser excluído.
    public async Task<bool> Excluir(int id)
    {
        var plano = await db.Planos.FindAsync(id);
        if (plano is null) return false;
        if (await db.Alunos.AnyAsync(a => a.PlanoId == id))
            throw new RegraNegocioException("Não é possível excluir um plano que possui alunos matriculados.");
        db.Planos.Remove(plano);
        await db.SaveChangesAsync();
        return true;
    }

    private static void Validar(PlanoEntradaDto dto)
    {
        var erros = ValidadorPlano.Validar(dto.Nome, dto.ValorMensal, dto.DuracaoMeses);
        if (erros.Count > 0) throw new RegraNegocioException(erros);
    }
}
