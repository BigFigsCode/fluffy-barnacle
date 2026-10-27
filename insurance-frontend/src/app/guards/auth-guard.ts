
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);
  const token = sessionStorage.getItem('token');

  if (!token) {
    return router.createUrlTree(['/login']);
  }

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const currentTime = Math.floor(Date.now() / 1000);

    if (typeof payload.exp !== 'number' || payload.exp <= currentTime) {
      sessionStorage.removeItem('token');
      return router.createUrlTree(['/login']);
    }

    return true;
  } catch {
    sessionStorage.removeItem('token');
    return router.createUrlTree(['/login']);
  }
};
