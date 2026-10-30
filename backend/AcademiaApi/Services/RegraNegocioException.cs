using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AcademiaApi.Services;

// Lançada pelos Services quando a entrada fere uma regra de validação ou de negócio.
public class RegraNegocioException(IEnumerable<string> erros) : Exception(string.Join(" ", erros))
{
    public List<string> Erros { get; } = erros.ToList();
    public RegraNegocioException(string erro) : this([erro]) { }
}

// Filtro registrado no Program.cs: transforma RegraNegocioException em 400 { "erros": [...] }.
public class RegraNegocioFiltro : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not RegraNegocioException regra) return;
        context.Result = new BadRequestObjectResult(new { erros = regra.Erros });
        context.ExceptionHandled = true;
    }
}
