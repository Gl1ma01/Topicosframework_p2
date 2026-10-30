export interface Plano { id: number; nome: string; valorMensal: number; duracaoMeses: number; quantidadeAlunos: number; }
export interface PlanoEntrada { nome: string; valorMensal: number; duracaoMeses: number; }

export interface Aluno { id: number; nome: string; cpf: string; email: string; dataNascimento: string; planoId: number; nomePlano: string; telefone: string | null; }
export interface AlunoEntrada { nome: string; cpf: string; email: string; dataNascimento: string | null; planoId: number; telefone: string; }

export interface Mensalidade { id: number; alunoId: number; valor: number; vencimento: string; dataPagamento: string | null; status: string; }
export interface MensalidadeEntrada { alunoId: number; valor: number; vencimento: string | null; }

export interface Inadimplente { alunoId: number; nomeAluno: string; mensalidadeId: number; valor: number; vencimento: string; diasAtraso: number; }
export interface RelatorioFinanceiro {
  ano: number; mes: number; totalPrevisto: number; totalRecebido: number; totalEmAberto: number;
  quantidadePagas: number; quantidadeEmAberto: number; inadimplentes: Inadimplente[];
}
