import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup } from '@angular/forms';
import { catchError, map, Observable, tap, of } from 'rxjs'; 
import {SecretData} from '../../../environment';

/**
 * Data to register a new user
 */
export interface RegisterUser{
  username: string,
  email: string,
  password: string,
}

/**
 * Data to login an existing user
 */
export interface LoginUser {
  email: string,
  password: string
}

/**
 * Response data from an authenticated user
 */
export interface AuthResponse{
  message?: string,
  isAuthenticated: boolean,
  username: string,
  email: string,
}

/**
 * Response message for logging out a user
 */
export interface LogoutResponse {
  message: string
}

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {

  /**
   * The current user state. EIther the user is authenticated or not (null).
   */
  public currentUser = signal<AuthResponse|null>(null);

  //The url api endpoint for login and registeration
  private registerUserURL = `${SecretData.baseuUrl}/api/users`;
  private loginUserURL = `${SecretData.baseuUrl}/api/users/token`;
  private logoutUserUrl = `${SecretData.baseuUrl}/logout`;
  private checkAuthUrl = `${SecretData.baseuUrl}/me`;

  private http = inject(HttpClient);

  /**
   * 
   * @param registerForm registerForm for registering a new user
   * @returns an Observable of RegisterUser for the HTTP POST function
   */
   registerUser(registerForm: FormGroup):Observable<AuthResponse> {
    //Use of getRawValue() to return the object with strict types
    const formData = registerForm.getRawValue();

    const newUser: RegisterUser = {
      username: formData.username,
      email: formData.email,
      password: formData.password
    }

    return this.http.post<AuthResponse>(this.registerUserURL, newUser);
  }

  /**
   * 
   * @param loginForm loginForm for logging an exisiting user
   * @returns an Observable of AuthResponse for the HTTP POST function
   */
  loginUser(loginForm: FormGroup): Observable<AuthResponse> {
    const formData = loginForm.getRawValue();

    const credentials: LoginUser = {
      email: formData.email,
      password: formData.password
    }

    return this.http.post<AuthResponse>(this.loginUserURL, credentials);
  }

  /**
   * 
   * @returns the repsone message from logging out a user
   */
  logoutUser(): Observable<LogoutResponse>{
    return this.http.post<LogoutResponse>(this.logoutUserUrl, {});
  }

/**
 * 
 * @returns an observable boolean true if the current user is authenticated, false otherwise
 */
  checkAuthStatus(): Observable<boolean> {
    return this.http.get<AuthResponse>(this.checkAuthUrl).pipe(
      //tap is used to perform a side effect
      tap({
          next: (user) => this.currentUser.set(user)
      }),
      map(() => true),
      catchError((err) => {
        this.currentUser.set(null);
        return of (false);
      })
    );
  }

  /**
   * 
   * @returns true if the current user is authenticated, false otherwise
   */
  isLoggedIn(): boolean {
    return this.currentUser()? true: false;
  }
}
