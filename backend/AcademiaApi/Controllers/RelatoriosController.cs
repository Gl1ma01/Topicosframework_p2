using AcademiaApi.Modelos;
using AcademiaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaApi.Controllers;

[ApiController]
[Route("api/relatorios")]
public class RelatoriosController(RelatorioService servico) : ControllerBase
{
    // GET /api/relatorios/financeiro?ano=2026&mes=10 (sem parâmetros = mês atual)
    [HttpGet("financeiro")]
    public async Task<ActionResult<RelatorioFinanceiroDto>> Financeiro(int? ano, int? mes) =>
        Ok(await servico.Financeiro(ano, mes));
}
