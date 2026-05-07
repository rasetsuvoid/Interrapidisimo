import { inject } from '@angular/core';
import { CanActivateChildFn, CanActivateFn, Router } from '@angular/router';
import { AuthSessionService } from '../services/auth-session.service';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthSessionService);
  const router = inject(Router);

  return auth.isAuthenticated() ? true : router.createUrlTree(['/auth']);
};

export const authChildGuard: CanActivateChildFn = () => {
  const auth = inject(AuthSessionService);
  const router = inject(Router);

  return auth.isAuthenticated() ? true : router.createUrlTree(['/auth']);
};

export const guestGuard: CanActivateFn = () => {
  const auth = inject(AuthSessionService);
  const router = inject(Router);

  return auth.isAuthenticated() ? router.createUrlTree(['/dashboard']) : true;
};
