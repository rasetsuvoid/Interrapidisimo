import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthSessionService } from '../services/auth-session.service';
import { ToastService } from '../services/toast.service';
import { RateLimitService } from '../services/rate-limit.service';

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast     = inject(ToastService);
  const session   = inject(AuthSessionService);
  const rateLimit = inject(RateLimitService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 0) {
        toast.error('No se pudo conectar con el servidor. Verifica tu conexión.');
      } else if (error.status === 429) {
        const msg: string =
          error.error?.errors?.[0] ??
          error.error?.message ??
          'Demasiadas solicitudes. Espera un momento antes de intentar de nuevo.';
        rateLimit.show(msg);
      } else if (error.status === 401 && !req.url.endsWith('/auth/login') && !req.url.endsWith('/auth/register')) {
        session.clear();
        toast.warning('Tu sesión expiró. Inicia sesión nuevamente.');
      } else if (error.status >= 500) {
        toast.error('Error del servidor. Inténtalo de nuevo más tarde.');
      }
      return throwError(() => error);
    })
  );
};
