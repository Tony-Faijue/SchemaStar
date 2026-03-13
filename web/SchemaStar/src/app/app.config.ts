import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './http-interceptors/auth-interceptor';
import { AuthenticationService } from './services/authentication-service';
import { lastValueFrom } from 'rxjs';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor]) //custom Http Interceptor
    ),
    //App Initializer to persist the global authenticated state after page reloads 
    provideAppInitializer(() => {
      const authService = inject(AuthenticationService);
      return lastValueFrom(authService.checkAuthStatus());
    }) 
  ]
};
