using Academia.Validacao;

namespace Academia.Validacao.Testes;

public class ValidadorMensalidadeTestes
{
    [Fact]
    public void Validar_MensalidadeValida_NaoRetornaErros()
    {
        // Arrange
        var vencimento = new DateOnly(2026, 11, 10);

        // Act
        var erros = ValidadorMensalidade.Validar(120m, vencimento, null);

        // Assert
        Assert.Empty(erros);
    }

    [Fact]
    public void Validar_ValorZero_RetornaErro()
    {
        // Arrange
        var vencimento = new DateOnly(2026, 11, 10);

        // Act
        var erros = ValidadorMensalidade.Validar(0m, vencimento, null);

        // Assert
        Assert.Contains("O valor da mensalidade deve ser maior que zero.", erros);
    }

    [Theory]
    [InlineData(1000000.01)]
    [InlineData(7e28)]
    public void Validar_ValorAcimaDoTeto_RetornaErro(decimal valor)
    {
        // Arrange
        var vencimento = new DateOnly(2026, 11, 10);

        // Act
        var erros = ValidadorMensalidade.Validar(valor, vencimento, null);

        // Assert
        Assert.Contains("O valor da mensalidade deve ser no máximo R$ 1.000.000,00.", erros);
    }
}
