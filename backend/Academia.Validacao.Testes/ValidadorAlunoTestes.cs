using Academia.Validacao;

namespace Academia.Validacao.Testes;

public class ValidadorAlunoTestes
{
    private static readonly DateOnly Hoje = new(2026, 10, 30);

    [Fact]
    public void Validar_AlunoValido_NaoRetornaErros()
    {
        // Arrange
        var nascimento = new DateOnly(2000, 5, 10);

        // Act
        var erros = ValidadorAluno.Validar("Maria Souza", "529.982.247-25", "maria@email.com", nascimento, Hoje);

        // Assert
        Assert.Empty(erros);
    }

    [Theory]
    [InlineData("52998224725", true)]
    [InlineData("529.982.247-25", true)]
    [InlineData("11111111111", false)]
    [InlineData("52998224724", false)]
    [InlineData("123", false)]
    [InlineData(null, false)]
    public void CpfValido_VariosCpfs_RetornaEsperado(string? cpf, bool esperado)
    {
        // Arrange (dados no InlineData)

        // Act
        var resultado = ValidadorAluno.CpfValido(cpf);

        // Assert
        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Jo")]
    [InlineData("   ")]
    public void Validar_NomeInvalido_RetornaErroDeNome(string nome)
    {
        // Arrange
        var nascimento = new DateOnly(2000, 1, 1);

        // Act
        var erros = ValidadorAluno.Validar(nome, "52998224725", "a@b.com", nascimento, Hoje);

        // Assert
        Assert.Contains("O nome do aluno deve ter entre 3 e 100 caracteres.", erros);
    }

    [Fact]
    public void Validar_EmailSemArroba_RetornaErroDeEmail()
    {
        // Arrange
        var nascimento = new DateOnly(2000, 1, 1);

        // Act
        var erros = ValidadorAluno.Validar("Maria Souza", "52998224725", "maria.email.com", nascimento, Hoje);

        // Assert
        Assert.Contains("E-mail inválido.", erros);
    }

    [Fact]
    public void Validar_AlunoMenorDe14Anos_RetornaErroDeIdade()
    {
        // Arrange
        var nascimento = new DateOnly(2013, 1, 1);

        // Act
        var erros = ValidadorAluno.Validar("Pedro Lima", "52998224725", "p@b.com", nascimento, Hoje);

        // Assert
        Assert.Contains("O aluno deve ter no mínimo 14 anos.", erros);
    }

    [Fact]
    public void CalcularIdade_AniversarioAindaNaoChegou_DescontaUmAno()
    {
        // Arrange
        var nascimento = new DateOnly(2000, 12, 25);

        // Act
        var idade = ValidadorAluno.CalcularIdade(nascimento, Hoje);

        // Assert
        Assert.Equal(25, idade);
    }
}
