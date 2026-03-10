import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthenticationService } from '../services/authentication-service';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  
  const authService = inject(AuthenticationService);
  const router = inject(Router);

  //If current user is authenticated let them access the route
  if(authService.isLoggedIn()){
    return true;
  }

  //Else ask if the credentials are still valid
  return authService.checkAuthStatus().pipe(
    map(isAuth => isAuth ? true : router.parseUrl('/login')) //false redirect to login page
  );
};
