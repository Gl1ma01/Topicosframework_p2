using System.Text.RegularExpressions;

namespace Academia.Validacao;

public static class ValidadorAluno
{
    public const int IdadeMinima = 14;

    public static List<string> Validar(string? nome, string? cpf, string? email, DateOnly dataNascimento, DateOnly hoje)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 3 || nome.Trim().Length > 100)
            erros.Add("O nome do aluno deve ter entre 3 e 100 caracteres.");

        if (!CpfValido(cpf))
            erros.Add("CPF inválido.");

        if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            erros.Add("E-mail inválido.");

        if (CalcularIdade(dataNascimento, hoje) < IdadeMinima)
            erros.Add($"O aluno deve ter no mínimo {IdadeMinima} anos.");

        return erros;
    }

    // Adicionado na 1.1.0. Telefone é opcional; se informado, precisa ter DDD + número (10 ou 11 dígitos).
    public static List<string> ValidarTelefone(string? telefone)
    {
        var erros = new List<string>();
        if (string.IsNullOrWhiteSpace(telefone)) return erros;

        var digitos = telefone.Count(char.IsDigit);
        if (digitos is < 10 or > 11 || telefone.Any(c => char.IsLetter(c)))
            erros.Add("Telefone inválido. Use DDD + número, ex.: (11) 98765-4321.");

        return erros;
    }

    public static int CalcularIdade(DateOnly dataNascimento, DateOnly hoje)
    {
        var idade = hoje.Year - dataNascimento.Year;
        if (dataNascimento > hoje.AddYears(-idade)) idade--;
        return idade;
    }

    public static bool CpfValido(string? cpf)
    {
        if (cpf is null) return false;
        var d = new string(cpf.Where(char.IsDigit).ToArray());
        if (d.Length != 11 || d.Distinct().Count() == 1) return false;

        for (var tamanho = 9; tamanho <= 10; tamanho++)
        {
            var soma = 0;
            for (var i = 0; i < tamanho; i++)
                soma += (d[i] - '0') * (tamanho + 1 - i);
            var resto = soma * 10 % 11 % 10;
            if (resto != d[tamanho] - '0') return false;
        }
        return true;
    }
}
