using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AcademiaApi.Dados;
using AcademiaApi.Modelos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AcademiaApi.Services;

public class AuthService(AcademiaContext db, IConfiguration config)
{
    // null = login ou senha incorretos (vira 401 no controller)
    public async Task<TokenDto?> Login(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Login) || string.IsNullOrWhiteSpace(dto.Senha))
            throw new RegraNegocioException("Informe login e senha.");

        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Login == dto.Login);
        if (usuario is null) return null;

        var resultado = new PasswordHasher<Usuario>().VerifyHashedPassword(usuario, usuario.SenhaHash, dto.Senha);
        if (resultado == PasswordVerificationResult.Failed) return null;

        var expira = DateTime.UtcNow.AddMinutes(config.GetValue<int>("Jwt:ExpiracaoMinutos"));
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Chave"]!));
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Emissor"],
            audience: config["Jwt:Publico"],
            claims: [new Claim(ClaimTypes.Name, usuario.Login)],
            expires: expira,
            signingCredentials: new SigningCredentials(chave, SecurityAlgorithms.HmacSha256));

        return new TokenDto(new JwtSecurityTokenHandler().WriteToken(token), expira);
    }
}
