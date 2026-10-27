
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const adminGuard: CanActivateFn = () => {
  const router = inject(Router);
  const token = sessionStorage.getItem('token');

  if (!token) {
    return router.createUrlTree(['/login']);
  }

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));

    const role =
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      ?? payload.role;

    if (role === 'Admin') {
      return true;
    }

    return router.createUrlTree(['/products']);
  } catch {
    return router.createUrlTree(['/login']);
  }
};
