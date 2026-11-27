namespace Academia.Validacao;

public static class ValidadorMensalidade
{
    public const decimal ValorMaximo = 1_000_000m;

    public static List<string> Validar(decimal valor, DateOnly vencimento, DateOnly? dataPagamento)
    {
        var erros = new List<string>();

        if (valor <= 0)
            erros.Add("O valor da mensalidade deve ser maior que zero.");
        else if (valor > ValorMaximo)
            erros.Add("O valor da mensalidade deve ser no máximo R$ 1.000.000,00.");

        if (vencimento.Year < 2000)
            erros.Add("Data de vencimento inválida.");

        if (dataPagamento is not null && dataPagamento.Value.Year < 2000)
            erros.Add("Data de pagamento inválida.");

        return erros;
    }
}
