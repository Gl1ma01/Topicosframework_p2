import { HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService, mensagensDeErro } from '../servicos/api.service';
import { AlunoEntrada, Plano } from '../servicos/modelos';

@Component({
  selector: 'app-aluno-form',
  imports: [FormsModule, RouterLink],
  template: `
    <h1>{{ id ? 'Editar aluno' : 'Novo aluno' }}</h1>
    @if (carregado()) {
    <form (ngSubmit)="salvar()">
      <label>Nome <input name="nome" [(ngModel)]="aluno.nome" /></label>
      <label>CPF <input name="cpf" [(ngModel)]="aluno.cpf" placeholder="000.000.000-00" /></label>
      <label>E-mail <input name="email" type="email" [(ngModel)]="aluno.email" /></label>
      <label>Telefone (opcional) <input name="telefone" [(ngModel)]="aluno.telefone" placeholder="(11) 98765-4321" /></label>
      <label>Data de nascimento <input name="dataNascimento" type="date" [(ngModel)]="aluno.dataNascimento" /></label>
      <label>Plano
        <select name="planoId" [(ngModel)]="aluno.planoId">
          @for (p of planos(); track p.id) { <option [ngValue]="p.id">{{ p.nome }} (R$ {{ p.valorMensal }})</option> }
        </select>
      </label>
      @if (erros().length) {
        <div class="erros">@for (e of erros(); track e) { <div>{{ e }}</div> }</div>
      }
      <div class="acoes">
        <button type="submit" [disabled]="salvando()">Salvar</button>
        <a class="botao secundario" routerLink="/alunos">Cancelar</a>
      </div>
    </form>
    }
  `,
})
export class AlunoForm implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  id = Number(inject(ActivatedRoute).snapshot.paramMap.get('id')) || null;
  aluno: AlunoEntrada = { nome: '', cpf: '', email: '', telefone: '', dataNascimento: '', planoId: 0 };
  planos = signal<Plano[]>([]);
  erros = signal<string[]>([]);
  salvando = signal(false);
  carregado = signal(!this.id);

  ngOnInit() {
    this.api.listarPlanos().subscribe({
      next: p => { this.planos.set(p); if (!this.aluno.planoId && p.length) this.aluno.planoId = p[0].id; },
      error: (e: HttpErrorResponse) => this.erros.set(mensagensDeErro(e)),
    });
    if (this.id) {
      this.api.obterAluno(this.id).subscribe({
        next: a => {
          this.aluno = { nome: a.nome, cpf: a.cpf, email: a.email, telefone: a.telefone ?? "", dataNascimento: a.dataNascimento, planoId: a.planoId };
          this.carregado.set(true); // signal avisa o Angular (sem zone.js) para redesenhar
        },
        error: (e: HttpErrorResponse) => this.erros.set(mensagensDeErro(e)),
      });
    }
  }

  salvar() {
    this.salvando.set(true);
    const dados = { ...this.aluno, dataNascimento: this.aluno.dataNascimento || null }; // vazio vai como null para a API responder com mensagem clara
    const requisicao: Observable<unknown> = this.id ? this.api.atualizarAluno(this.id, dados) : this.api.criarAluno(dados);
    requisicao.subscribe({
      next: () => this.router.navigate(['/alunos']),
      error: (e: HttpErrorResponse) => { this.erros.set(mensagensDeErro(e)); this.salvando.set(false); },
    });
  }
}
