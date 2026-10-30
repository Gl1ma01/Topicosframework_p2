using AcademiaApi.Modelos;
using AcademiaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService servico) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login(LoginDto dto)
    {
        var token = await servico.Login(dto);
        return token is null ? Unauthorized(new { erros = new[] { "Login ou senha inválidos." } }) : Ok(token);
    }
}
