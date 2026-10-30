import { HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService, mensagensDeErro } from '../servicos/api.service';
import { Plano, PlanoEntrada } from '../servicos/modelos';

// CRUD de planos na mesma tela: a tabela lista, o formulário cria ou edita.
@Component({
  selector: 'app-planos',
  imports: [FormsModule, CurrencyPipe],
  template: `
    <h1>Planos</h1>
    @if (erros().length) {
      <div class="erros">@for (e of erros(); track e) { <div>{{ e }}</div> }</div>
    }
    <table>
      <thead><tr><th>Nome</th><th>Valor mensal</th><th>Duração</th><th>Alunos</th><th></th></tr></thead>
      <tbody>
        @for (p of planos(); track p.id) {
          <tr>
            <td>{{ p.nome }}</td>
            <td>{{ p.valorMensal | currency: 'BRL' }}</td>
            <td>{{ p.duracaoMeses }} {{ p.duracaoMeses === 1 ? 'mês' : 'meses' }}</td>
            <td>{{ p.quantidadeAlunos }}</td>
            <td class="acoes">
              <button class="secundario" (click)="editar(p)">Editar</button>
              <button class="perigo" (click)="excluir(p)">Excluir</button>
            </td>
          </tr>
        }
      </tbody>
    </table>

    <h2>{{ editandoId() ? 'Editar plano' : 'Novo plano' }}</h2>
    <form (ngSubmit)="salvar()">
      <label>Nome <input name="nome" [(ngModel)]="form.nome" /></label>
      <label>Valor mensal (R$) <input name="valorMensal" type="number" step="0.01" [(ngModel)]="form.valorMensal" /></label>
      <label>Duração (meses) <input name="duracaoMeses" type="number" [(ngModel)]="form.duracaoMeses" /></label>
      <div class="acoes">
        <button type="submit">Salvar</button>
        @if (editandoId()) { <button type="button" class="secundario" (click)="limpar()">Cancelar</button> }
      </div>
    </form>
  `,
})
export class Planos implements OnInit {
  private api = inject(ApiService);
  planos = signal<Plano[]>([]);
  erros = signal<string[]>([]);
  editandoId = signal<number | null>(null);
  form: PlanoEntrada = { nome: '', valorMensal: 0, duracaoMeses: 1 };

  private falhou = (e: HttpErrorResponse) => this.erros.set(mensagensDeErro(e));

  ngOnInit() { this.carregar(); }

  carregar() { this.api.listarPlanos().subscribe({ next: l => this.planos.set(l), error: this.falhou }); }

  editar(p: Plano) {
    this.form = { nome: p.nome, valorMensal: p.valorMensal, duracaoMeses: p.duracaoMeses };
    this.editandoId.set(p.id);
  }

  limpar() {
    this.form = { nome: '', valorMensal: 0, duracaoMeses: 1 };
    this.editandoId.set(null);
    this.erros.set([]);
  }

  salvar() {
    const id = this.editandoId();
    // campo numérico vazio vira 0 para a biblioteca de validação responder com a mensagem dela
    const dados = { ...this.form, valorMensal: Number(this.form.valorMensal) || 0, duracaoMeses: Number(this.form.duracaoMeses) || 0 };
    const requisicao: Observable<unknown> = id ? this.api.atualizarPlano(id, dados) : this.api.criarPlano(dados);
    requisicao.subscribe({ next: () => { this.limpar(); this.carregar(); }, error: this.falhou });
  }

  excluir(p: Plano) {
    if (!confirm(`Excluir o plano ${p.nome}?`)) return;
    this.api.excluirPlano(p.id).subscribe({ next: () => { this.erros.set([]); this.carregar(); }, error: this.falhou });
  }
}
