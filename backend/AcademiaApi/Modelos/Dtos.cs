namespace AcademiaApi.Modelos;

// Strings anuláveis de propósito: quem valida é a biblioteca Academia.Validacao, com mensagens em português.
public record LoginDto(string? Login, string? Senha);
public record TokenDto(string Token, DateTime ExpiraEm);

public record PlanoEntradaDto(string? Nome, decimal ValorMensal, int DuracaoMeses);
public record PlanoDto(int Id, string Nome, decimal ValorMensal, int DuracaoMeses, int QuantidadeAlunos);

public record AlunoEntradaDto(string? Nome, string? Cpf, string? Email, DateOnly DataNascimento, int PlanoId, string? Telefone);
public record AlunoDto(int Id, string Nome, string Cpf, string Email, DateOnly DataNascimento, int PlanoId, string NomePlano, string? Telefone);

public record MensalidadeEntradaDto(int AlunoId, decimal Valor, DateOnly Vencimento);
public record MensalidadeDto(int Id, int AlunoId, decimal Valor, DateOnly Vencimento, DateOnly? DataPagamento, string Status);

public record InadimplenteDto(int AlunoId, string NomeAluno, int MensalidadeId, decimal Valor, DateOnly Vencimento, int DiasAtraso);
public record RelatorioFinanceiroDto(int Ano, int Mes, decimal TotalPrevisto, decimal TotalRecebido, decimal TotalEmAberto,
    int QuantidadePagas, int QuantidadeEmAberto, List<InadimplenteDto> Inadimplentes);
