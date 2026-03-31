import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { GlobalErrorNotificationService } from '../services/global-error-notification-service';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const globalErrorInterceptor: HttpInterceptorFn = (req, next) => {

  const router = inject(Router);
  const globalErrorNotificationService = inject(GlobalErrorNotificationService);
  
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const message = extractErrorMessage(error);

      if (error.status === 401){
        //Let login component handle the error
        if (router.url === '/login'){
          return throwError(() => error);
        }
        //Otherwise the session expired
        router.navigate(['/login']);
      }
      //Notify globally for non-login errors
      else {
        globalErrorNotificationService.showError(message);
      }

      return throwError(() => error);
    })
  );
};

/**
 * 
 * @param err 
 * @returns an error message from the HTTPErrorResponse object
 */
export function extractErrorMessage(err: HttpErrorResponse): string {
  //Gets the problem details error message
  if (err.error && err.error.detail){
    return err.error.detail;
  }
  return 'An unexpected error occured. Please try again.';
}
