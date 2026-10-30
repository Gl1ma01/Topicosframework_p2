using AcademiaApi.Dados;
using AcademiaApi.Modelos;
using Microsoft.EntityFrameworkCore;

namespace AcademiaApi.Services;

public class RelatorioService(AcademiaContext db)
{
    // Relatório financeiro do mês: previsto x recebido x em aberto + lista de inadimplentes.
    // Sem ano/mês informados, usa o mês atual.
    public async Task<RelatorioFinanceiroDto> Financeiro(int? anoInformado, int? mesInformado)
    {
        var ano = anoInformado ?? DateTime.Today.Year;
        var mes = mesInformado ?? DateTime.Today.Month;
        if (mes is < 1 or > 12 || ano is < 2000 or > 2100)
            throw new RegraNegocioException("Informe um mês entre 1 e 12 e um ano entre 2000 e 2100.");

        var inicio = new DateOnly(ano, mes, 1);
        var fim = inicio.AddMonths(1);
        var hoje = DateOnly.FromDateTime(DateTime.Today);

        // Obs.: soma em memória porque o SQLite não soma decimal no banco; com muito dado, guardar centavos em int.
        var doMes = await db.Mensalidades.Include(m => m.Aluno)
            .Where(m => m.Vencimento >= inicio && m.Vencimento < fim)
            .ToListAsync();

        var pagas = doMes.Where(m => m.DataPagamento is not null).ToList();
        var abertas = doMes.Where(m => m.DataPagamento is null).ToList();

        var inadimplentes = abertas.Where(m => m.Vencimento < hoje)
            .OrderBy(m => m.Vencimento)
            .Select(m => new InadimplenteDto(m.AlunoId, m.Aluno!.Nome, m.Id, m.Valor, m.Vencimento, hoje.DayNumber - m.Vencimento.DayNumber))
            .ToList();

        return new RelatorioFinanceiroDto(ano, mes,
            doMes.Sum(m => m.Valor), pagas.Sum(m => m.Valor), abertas.Sum(m => m.Valor),
            pagas.Count, abertas.Count, inadimplentes);
    }
}
