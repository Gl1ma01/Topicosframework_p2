import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Aluno, AlunoEntrada, Mensalidade, MensalidadeEntrada, Plano, PlanoEntrada, RelatorioFinanceiro } from './modelos';

export const API_URL = 'http://localhost:5080/api';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);

  listarPlanos() { return this.http.get<Plano[]>(`${API_URL}/planos`); }
  criarPlano(p: PlanoEntrada) { return this.http.post<Plano>(`${API_URL}/planos`, p); }
  atualizarPlano(id: number, p: PlanoEntrada) { return this.http.put<void>(`${API_URL}/planos/${id}`, p); }
  excluirPlano(id: number) { return this.http.delete<void>(`${API_URL}/planos/${id}`); }

  listarAlunos() { return this.http.get<Aluno[]>(`${API_URL}/alunos`); }
  obterAluno(id: number) { return this.http.get<Aluno>(`${API_URL}/alunos/${id}`); }
  criarAluno(a: AlunoEntrada) { return this.http.post<Aluno>(`${API_URL}/alunos`, a); }
  atualizarAluno(id: number, a: AlunoEntrada) { return this.http.put<void>(`${API_URL}/alunos/${id}`, a); }
  excluirAluno(id: number) { return this.http.delete<void>(`${API_URL}/alunos/${id}`); }

  mensalidadesDoAluno(alunoId: number) { return this.http.get<Mensalidade[]>(`${API_URL}/alunos/${alunoId}/mensalidades`); }
  criarMensalidade(m: MensalidadeEntrada) { return this.http.post<Mensalidade>(`${API_URL}/mensalidades`, m); }
  pagarMensalidade(id: number) { return this.http.put<Mensalidade>(`${API_URL}/mensalidades/${id}/pagar`, {}); }
  excluirMensalidade(id: number) { return this.http.delete<void>(`${API_URL}/mensalidades/${id}`); }

  relatorioFinanceiro(ano: number, mes: number) {
    return this.http.get<RelatorioFinanceiro>(`${API_URL}/relatorios/financeiro`, { params: { ano, mes } });
  }
}

// Converte o erro da API ({ erros: [...] }) em lista de mensagens para mostrar na tela.
export function mensagensDeErro(e: HttpErrorResponse): string[] {
  if (e.status === 0) return ['Não foi possível conectar à API. Ela está rodando em http://localhost:5080?'];
  if (Array.isArray(e.error?.erros)) return e.error.erros;
  if (e.status === 404) return ['Registro não encontrado.'];
  if (e.status === 401) return ['Sessão expirada. Faça login novamente.'];
  return [`Erro inesperado (HTTP ${e.status}).`];
}
