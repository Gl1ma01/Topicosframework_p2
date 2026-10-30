using AcademiaApi.Modelos;
using AcademiaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaApi.Controllers;

[ApiController]
[Route("api/planos")]
public class PlanosController(PlanoService servico) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PlanoDto>>> Listar() => Ok(await servico.Listar());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlanoDto>> Obter(int id)
    {
        var plano = await servico.Obter(id);
        return plano is null ? NotFound() : Ok(plano);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PlanoDto>> Criar(PlanoEntradaDto dto)
    {
        var plano = await servico.Criar(dto);
        return CreatedAtAction(nameof(Obter), new { id = plano.Id }, plano);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, PlanoEntradaDto dto) =>
        await servico.Atualizar(id, dto) ? NoContent() : NotFound();

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id) =>
        await servico.Excluir(id) ? NoContent() : NotFound();
}
