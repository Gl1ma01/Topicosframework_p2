using System.Text;
using AcademiaApi.Dados;
using AcademiaApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// EF Core + SQLite
builder.Services.AddDbContext<AcademiaContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Academia")));

// Injeção de dependência dos Services (recebidos pelo construtor dos controllers)
builder.Services.AddScoped<PlanoService>();
builder.Services.AddScoped<AlunoService>();
builder.Services.AddScoped<MensalidadeService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<AuthService>();

// RegraNegocioException vira 400 { erros: [...] } (RegraNegocioFiltro).
// Erros de modelo (ex.: JSON malformado) também saem como 400 { erros: [...] }.
builder.Services.AddControllers(o => o.Filters.Add<RegraNegocioFiltro>()).ConfigureApiBehaviorOptions(o =>
    o.InvalidModelStateResponseFactory = ctx => new BadRequestObjectResult(new
    {
        erros = new[] { "Requisição inválida: verifique o formato do JSON e os tipos dos campos." }
    }));

// JWT
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Emissor"],
        ValidAudience = jwt["Publico"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Chave"]!)),
        ClockSkew = TimeSpan.Zero // expira exatamente no tempo configurado
    });
builder.Services.AddAuthorization();

// CORS para o front Angular
builder.Services.AddCors(o => o.AddPolicy("Frontend", p => p
    .WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithExposedHeaders("Location")));

var app = builder.Build();

// Aplica as migrations e carrega o seed (só insere se o banco estiver vazio)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AcademiaContext>();
    db.Database.Migrate();
    SeedDados.Executar(db);
}

// Rede de segurança: falha inesperada vira 500 genérico, sem expor detalhes internos
app.UseExceptionHandler(erro => erro.Run(async ctx =>
{
    ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await ctx.Response.WriteAsJsonAsync(new { erros = new[] { "Erro interno no servidor." } });
}));

// POST/PUT sem Content-Type (ex.: curl sem -H) cairia em 415; tratamos como JSON para responder 400 com mensagem clara.
app.Use((ctx, next) =>
{
    if (ctx.Request.ContentType is null && (HttpMethods.IsPost(ctx.Request.Method) || HttpMethods.IsPut(ctx.Request.Method)))
        ctx.Request.ContentType = "application/json";
    return next();
});

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
