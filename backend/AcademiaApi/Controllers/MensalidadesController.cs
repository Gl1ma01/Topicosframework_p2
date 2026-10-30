using AcademiaApi.Modelos;
using AcademiaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaApi.Controllers;

[ApiController]
[Route("api/mensalidades")]
public class MensalidadesController(MensalidadeService servico) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MensalidadeDto>> Obter(int id)
    {
        var m = await servico.Obter(id);
        return m is null ? NotFound() : Ok(m);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<MensalidadeDto>> Criar(MensalidadeEntradaDto dto)
    {
        var m = await servico.Criar(dto);
        return CreatedAtAction(nameof(Obter), new { id = m.Id }, m);
    }

    [Authorize]
    [HttpPut("{id:int}/pagar")]
    public async Task<ActionResult<MensalidadeDto>> Pagar(int id)
    {
        var m = await servico.Pagar(id);
        return m is null ? NotFound() : Ok(m);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id) =>
        await servico.Excluir(id) ? NoContent() : NotFound();
}
