import { HttpErrorResponse } from '@angular/common/http';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService, mensagensDeErro } from '../servicos/api.service';
import { Aluno, Mensalidade } from '../servicos/modelos';

// Tela do relacionamento 1:N: um aluno e as mensalidades dele.
@Component({
  selector: 'app-aluno-detalhe',
  imports: [FormsModule, RouterLink, CurrencyPipe, DatePipe],
  template: `
    <a routerLink="/alunos">&larr; Voltar</a>
    @if (aluno(); as a) {
      <h1>{{ a.nome }}</h1>
      <p>Plano {{ a.nomePlano }} · {{ a.email }} @if (a.telefone) { · {{ a.telefone }} }</p>
    }

    @if (erros().length) {
      <div class="erros">@for (e of erros(); track e) { <div>{{ e }}</div> }</div>
    }

    <h2>Mensalidades</h2>
    <table>
      <thead><tr><th>Vencimento</th><th>Valor</th><th>Status</th><th>Pago em</th><th></th></tr></thead>
      <tbody>
        @for (m of mensalidades(); track m.id) {
          <tr>
            <td>{{ m.vencimento | date: 'dd/MM/yyyy' }}</td>
            <td>{{ m.valor | currency: 'BRL' }}</td>
            <td [class]="'status-' + m.status.replace(' ', '-')">{{ m.status }}</td>
            <td>{{ m.dataPagamento ? (m.dataPagamento | date: 'dd/MM/yyyy') : '—' }}</td>
            <td class="acoes">
              @if (!m.dataPagamento) {
                <button (click)="pagar(m)">Registrar pagamento</button>
                <button class="perigo" (click)="excluir(m)">Excluir</button>
              }
            </td>
          </tr>
        } @empty {
          <tr><td colspan="5">Nenhuma mensalidade.</td></tr>
        }
      </tbody>
    </table>

    <h2>Nova mensalidade</h2>
    <form (ngSubmit)="criar()">
      <label>Valor (R$) <input name="valor" type="number" step="0.01" [(ngModel)]="nova.valor" /></label>
      <label>Vencimento <input name="vencimento" type="date" [(ngModel)]="nova.vencimento" /></label>
      <button type="submit">Adicionar</button>
    </form>
  `,
})
export class AlunoDetalhe implements OnInit {
  private api = inject(ApiService);
  private alunoId = Number(inject(ActivatedRoute).snapshot.paramMap.get('id'));
  aluno = signal<Aluno | null>(null);
  mensalidades = signal<Mensalidade[]>([]);
  erros = signal<string[]>([]);
  nova = { valor: 0, vencimento: '' };

  private falhou = (e: HttpErrorResponse) => this.erros.set(mensagensDeErro(e));

  ngOnInit() {
    this.api.obterAluno(this.alunoId).subscribe({
      next: a => this.aluno.set(a),
      error: this.falhou,
    });
    this.carregar();
  }

  carregar() {
    this.api.mensalidadesDoAluno(this.alunoId).subscribe({ next: l => this.mensalidades.set(l), error: this.falhou });
  }

  criar() {
    this.api.criarMensalidade({ alunoId: this.alunoId, valor: Number(this.nova.valor) || 0, vencimento: this.nova.vencimento || null }).subscribe({
      next: () => { this.erros.set([]); this.nova = { valor: 0, vencimento: '' }; this.carregar(); },
      error: this.falhou,
    });
  }

  pagar(m: Mensalidade) {
    this.api.pagarMensalidade(m.id).subscribe({ next: () => { this.erros.set([]); this.carregar(); }, error: this.falhou });
  }

  excluir(m: Mensalidade) {
    if (!confirm('Excluir esta mensalidade?')) return;
    this.api.excluirMensalidade(m.id).subscribe({ next: () => { this.erros.set([]); this.carregar(); }, error: this.falhou });
  }
}
