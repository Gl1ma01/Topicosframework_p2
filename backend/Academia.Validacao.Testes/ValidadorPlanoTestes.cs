using Academia.Validacao;

namespace Academia.Validacao.Testes;

public class ValidadorPlanoTestes
{
    [Fact]
    public void Validar_PlanoValido_NaoRetornaErros()
    {
        // Arrange
        var nome = "Mensal";

        // Act
        var erros = ValidadorPlano.Validar(nome, 99.90m, 1);

        // Assert
        Assert.Empty(erros);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Validar_ValorMensalNaoPositivo_RetornaErro(decimal valor)
    {
        // Arrange
        var nome = "Mensal";

        // Act
        var erros = ValidadorPlano.Validar(nome, valor, 1);

        // Assert
        Assert.Contains("O valor mensal deve ser maior que zero.", erros);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(25)]
    public void Validar_DuracaoForaDoIntervalo_RetornaErro(int duracao)
    {
        // Arrange
        var nome = "Anual";

        // Act
        var erros = ValidadorPlano.Validar(nome, 80m, duracao);

        // Assert
        Assert.Contains("A duração do plano deve ser entre 1 e 24 meses.", erros);
    }
}
