namespace Academia.Validacao;

public static class ValidadorPlano
{
    public static List<string> Validar(string? nome, decimal valorMensal, int duracaoMeses)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 3)
            erros.Add("O nome do plano deve ter pelo menos 3 caracteres.");

        if (valorMensal <= 0)
            erros.Add("O valor mensal deve ser maior que zero.");

        if (duracaoMeses < 1 || duracaoMeses > 24)
            erros.Add("A duração do plano deve ser entre 1 e 24 meses.");

        return erros;
    }
}
