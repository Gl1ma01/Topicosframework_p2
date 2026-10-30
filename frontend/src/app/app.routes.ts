import { Routes } from '@angular/router';
import { authGuard } from './guardas/auth.guard';
import { Login } from './paginas/login';
import { AlunosLista } from './paginas/alunos-lista';
import { AlunoForm } from './paginas/aluno-form';
import { AlunoDetalhe } from './paginas/aluno-detalhe';
import { Planos } from './paginas/planos';
import { Relatorio } from './paginas/relatorio';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'alunos', component: AlunosLista, canActivate: [authGuard] },
  { path: 'alunos/novo', component: AlunoForm, canActivate: [authGuard] },
  { path: 'alunos/:id/editar', component: AlunoForm, canActivate: [authGuard] },
  { path: 'alunos/:id', component: AlunoDetalhe, canActivate: [authGuard] },
  { path: 'planos', component: Planos, canActivate: [authGuard] },
  { path: 'relatorio', component: Relatorio, canActivate: [authGuard] },
  { path: '', redirectTo: 'alunos', pathMatch: 'full' },
  { path: '**', redirectTo: 'alunos' }
];
