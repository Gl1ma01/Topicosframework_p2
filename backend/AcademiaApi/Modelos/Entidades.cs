namespace AcademiaApi.Modelos;

public class Plano
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public decimal ValorMensal { get; set; }
    public int DuracaoMeses { get; set; }
    public List<Aluno> Alunos { get; set; } = [];
}

public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Cpf { get; set; } = "";
    public string Email { get; set; } = "";
    public DateOnly DataNascimento { get; set; }
    public string? Telefone { get; set; } // adicionado na migration AdicionaTelefoneAluno
    public int PlanoId { get; set; }
    public Plano? Plano { get; set; }
    public List<Mensalidade> Mensalidades { get; set; } = [];
}

public class Mensalidade
{
    public int Id { get; set; }
    public int AlunoId { get; set; }
    public Aluno? Aluno { get; set; }
    public decimal Valor { get; set; }
    public DateOnly Vencimento { get; set; }
    public DateOnly? DataPagamento { get; set; }
}

public class Usuario
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string SenhaHash { get; set; } = "";
}
