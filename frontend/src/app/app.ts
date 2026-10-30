import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './servicos/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    @if (auth.logado()) {
      <header>
        <strong>Academia</strong>
        <nav>
          <a routerLink="/alunos" routerLinkActive="ativo">Alunos</a>
          <a routerLink="/planos" routerLinkActive="ativo">Planos</a>
          <a routerLink="/relatorio" routerLinkActive="ativo">Relatório</a>
        </nav>
        <button class="secundario" (click)="auth.sair()">Sair</button>
      </header>
    }
    <main><router-outlet /></main>
  `,
  styles: `
    header { display: flex; align-items: center; gap: 1.5rem; padding: .75rem 1.5rem; background: #1f2937; color: #fff; }
    nav { display: flex; gap: 1rem; flex: 1; }
    nav a { color: #cbd5e1; text-decoration: none; }
    nav a.ativo { color: #fff; font-weight: 600; }
    main { max-width: 960px; margin: 1.5rem auto; padding: 0 1rem; }
  `,
})
export class App {
  protected auth = inject(AuthService);
}
