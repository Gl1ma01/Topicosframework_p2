import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../servicos/auth.service';

// Bloqueia as telas para quem não está logado.
export const authGuard: CanActivateFn = () =>
  inject(AuthService).tokenValido() ? true : inject(Router).createUrlTree(['/login']);
