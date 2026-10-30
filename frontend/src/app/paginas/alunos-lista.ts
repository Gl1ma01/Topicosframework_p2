import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService, mensagensDeErro } from '../servicos/api.service';
import { Aluno } from '../servicos/modelos';

@Component({
  selector: 'app-alunos-lista',
  imports: [RouterLink],
  template: `
    <div class="topo">
      <h1>Alunos</h1>
      <a class="botao" routerLink="/alunos/novo">Novo aluno</a>
    </div>
    @if (erros().length) {
      <div class="erros">@for (e of erros(); track e) { <div>{{ e }}</div> }</div>
    }
    <table>
      <thead><tr><th>Nome</th><th>E-mail</th><th>Plano</th><th></th></tr></thead>
      <tbody>
        @for (a of alunos(); track a.id) {
          <tr>
            <td><a [routerLink]="['/alunos', a.id]">{{ a.nome }}</a></td>
            <td>{{ a.email }}</td>
            <td>{{ a.nomePlano }}</td>
            <td class="acoes">
              <a class="botao" [routerLink]="['/alunos', a.id]">Mensalidades</a>
              <a class="botao secundario" [routerLink]="['/alunos', a.id, 'editar']">Editar</a>
              <button class="perigo" (click)="excluir(a)">Excluir</button>
            </td>
          </tr>
        } @empty {
          <tr><td colspan="4">Nenhum aluno cadastrado.</td></tr>
        }
      </tbody>
    </table>
  `,
})
export class AlunosLista implements OnInit {
  private api = inject(ApiService);
  alunos = signal<Aluno[]>([]);
  erros = signal<string[]>([]);

  ngOnInit() { this.carregar(); }

  carregar() {
    this.api.listarAlunos().subscribe({
      next: lista => this.alunos.set(lista),
      error: (e: HttpErrorResponse) => this.erros.set(mensagensDeErro(e)),
    });
  }

  excluir(a: Aluno) {
    if (!confirm(`Excluir ${a.nome} e todas as mensalidades dele?`)) return;
    this.api.excluirAluno(a.id).subscribe({
      next: () => { this.erros.set([]); this.carregar(); },
      error: (e: HttpErrorResponse) => this.erros.set(mensagensDeErro(e)),
    });
  }
}
