using AcademiaApi.Modelos;
using AcademiaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaApi.Controllers;

[ApiController]
[Route("api/alunos")]
public class AlunosController(AlunoService servico, MensalidadeService mensalidades) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AlunoDto>>> Listar() => Ok(await servico.Listar());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AlunoDto>> Obter(int id)
    {
        var aluno = await servico.Obter(id);
        return aluno is null ? NotFound() : Ok(aluno);
    }

    // Relacionamento 1:N: mensalidades de um aluno
    [HttpGet("{id:int}/mensalidades")]
    public async Task<ActionResult<List<MensalidadeDto>>> Mensalidades(int id)
    {
        var lista = await mensalidades.ListarDoAluno(id);
        return lista is null ? NotFound() : Ok(lista);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AlunoDto>> Criar(AlunoEntradaDto dto)
    {
        var aluno = await servico.Criar(dto);
        return CreatedAtAction(nameof(Obter), new { id = aluno.Id }, aluno);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, AlunoEntradaDto dto) =>
        await servico.Atualizar(id, dto) ? NoContent() : NotFound();

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id) =>
        await servico.Excluir(id) ? NoContent() : NotFound();
}
