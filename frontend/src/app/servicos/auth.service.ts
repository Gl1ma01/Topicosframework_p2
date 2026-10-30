import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { API_URL } from './api.service';

interface TokenResposta { token: string; expiraEm: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  readonly logado = signal(this.tokenValido());

  login(login: string, senha: string) {
    return this.http.post<TokenResposta>(`${API_URL}/auth/login`, { login, senha }).pipe(
      tap(r => {
        localStorage.setItem('token', r.token);
        localStorage.setItem('expiraEm', r.expiraEm);
        this.logado.set(true);
      }));
  }

  sair() {
    localStorage.removeItem('token');
    localStorage.removeItem('expiraEm');
    this.logado.set(false);
    this.router.navigate(['/login']);
  }

  token() { return this.tokenValido() ? localStorage.getItem('token') : null; }

  tokenValido() {
    const expira = localStorage.getItem('expiraEm');
    return !!localStorage.getItem('token') && !!expira && new Date(expira) > new Date();
  }
}
