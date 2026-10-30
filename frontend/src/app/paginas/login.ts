import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { mensagensDeErro } from '../servicos/api.service';
import { AuthService } from '../servicos/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  template: `
    <h1>Entrar</h1>
    <form (ngSubmit)="entrar()">
      <label>Login <input name="login" [(ngModel)]="login" autocomplete="username" required /></label>
      <label>Senha <input name="senha" type="password" [(ngModel)]="senha" autocomplete="current-password" required /></label>
      @if (erros().length) {
        <div class="erros">@for (e of erros(); track e) { <div>{{ e }}</div> }</div>
      }
      <button type="submit" [disabled]="carregando()">Entrar</button>
    </form>
  `,
})
export class Login {
  private auth = inject(AuthService);
  private router = inject(Router);
  login = '';
  senha = '';
  erros = signal<string[]>([]);
  carregando = signal(false);

  entrar() {
    this.carregando.set(true);
    this.auth.login(this.login, this.senha).subscribe({
      next: () => this.router.navigate(['/alunos']),
      error: (e: HttpErrorResponse) => { this.erros.set(mensagensDeErro(e)); this.carregando.set(false); },
    });
  }
}
