import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../servicos/auth.service';

// Anexa o JWT em toda requisição; se a API responder 401, desloga.
export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.token();
  const requisicao = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;

  return next(requisicao).pipe(catchError((e: HttpErrorResponse) => {
    if (e.status === 401 && !req.url.endsWith('/auth/login')) auth.sair();
    return throwError(() => e);
  }));
};
