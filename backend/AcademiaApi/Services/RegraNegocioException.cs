namespace AcademiaApi.Services;

// Lançada pelos Services; o Program.cs transforma em 400 { "erros": [...] }.
public class RegraNegocioException(IEnumerable<string> erros) : Exception(string.Join(" ", erros))
{
    public List<string> Erros { get; } = erros.ToList();
    public RegraNegocioException(string erro) : this([erro]) { }
}
