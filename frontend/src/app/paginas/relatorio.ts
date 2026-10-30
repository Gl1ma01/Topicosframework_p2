import { HttpErrorResponse } from '@angular/common/http';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService, mensagensDeErro } from '../servicos/api.service';
import { RelatorioFinanceiro } from '../servicos/modelos';

// Relatório calculado pela API: previsto x recebido x em aberto e inadimplentes do mês.
@Component({
  selector: 'app-relatorio',
  imports: [FormsModule, RouterLink, CurrencyPipe, DatePipe],
  template: `
    <h1>Relatório financeiro</h1>
    <div class="acoes">
      <label>Mês <input name="mes" type="number" min="1" max="12" [(ngModel)]="mes" /></label>
      <label>Ano <input name="ano" type="number" [(ngModel)]="ano" /></label>
      <button (click)="carregar()">Gerar</button>
    </div>

    @if (erros().length) {
      <div class="erros">@for (e of erros(); track e) { <div>{{ e }}</div> }</div>
    }

    @if (relatorio(); as r) {
      <div class="cards">
        <div class="card"><span>Previsto</span><strong>{{ r.totalPrevisto | currency: 'BRL' }}</strong></div>
        <div class="card"><span>Recebido ({{ r.quantidadePagas }})</span><strong>{{ r.totalRecebido | currency: 'BRL' }}</strong></div>
        <div class="card"><span>Em aberto ({{ r.quantidadeEmAberto }})</span><strong>{{ r.totalEmAberto | currency: 'BRL' }}</strong></div>
      </div>
      <h2>Inadimplentes</h2>
      <table>
        <thead><tr><th>Aluno</th><th>Vencimento</th><th>Valor</th><th>Dias de atraso</th></tr></thead>
        <tbody>
          @for (i of r.inadimplentes; track i.mensalidadeId) {
            <tr>
              <td><a [routerLink]="['/alunos', i.alunoId]">{{ i.nomeAluno }}</a></td>
              <td>{{ i.vencimento | date: 'dd/MM/yyyy' }}</td>
              <td>{{ i.valor | currency: 'BRL' }}</td>
              <td>{{ i.diasAtraso }}</td>
            </tr>
          } @empty {
            <tr><td colspan="4">Nenhum inadimplente neste mês.</td></tr>
          }
        </tbody>
      </table>
    }
  `,
})
export class Relatorio implements OnInit {
  private api = inject(ApiService);
  mes = new Date().getMonth() + 1;
  ano = new Date().getFullYear();
  relatorio = signal<RelatorioFinanceiro | null>(null);
  erros = signal<string[]>([]);

  ngOnInit() { this.carregar(); }

  carregar() {
    this.api.relatorioFinanceiro(this.ano, this.mes).subscribe({
      next: r => { this.relatorio.set(r); this.erros.set([]); },
      error: (e: HttpErrorResponse) => { this.relatorio.set(null); this.erros.set(mensagensDeErro(e)); },
    });
  }
}
