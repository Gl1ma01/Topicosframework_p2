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
}
