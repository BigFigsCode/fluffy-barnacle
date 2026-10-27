
import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = sessionStorage.getItem('token');

  const isLocalApi = req.url.startsWith('http://localhost:5085/api/');
  const isAzureApi = req.url.startsWith(
    'https://insurance-policy-management-webapi-cudchfg2dpfzdkag.australiaeast-01.azurewebsites.net/api/'
  );

  if (token && (isLocalApi || isAzureApi)) {
    const authRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });

    return next(authRequest);
  }

  return next(req);
};
