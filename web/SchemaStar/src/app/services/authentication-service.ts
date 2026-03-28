import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
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
 * Response data from an authenticated user with cookie
 */
export interface AuthResponse{
  isAuthenticated: boolean,
  publicId: string,
  username: string,
  email: string,
  createdAt: string,
  updatedAt: string
}

/**
 * Response data for user data
 */
export interface UserResponse {
  publicId: string,
  username: string,
  email: string,
  createdAt: string,
  updatedAt: string
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
  private logoutUserUrl = `${SecretData.baseuUrl}/api/users/logout`;
  private checkAuthUrl = `${SecretData.baseuUrl}/api/users/me`;

  private http = inject(HttpClient);

  /**
   * 
   * @param credentials credentials for registering a new user
   * @returns an Observable of UserResponse for the HTTP POST function
   */
   registerUser(credentials: RegisterUser):Observable<UserResponse> {
    return this.http.post<UserResponse>(this.registerUserURL, credentials);
  }

  /**
   * 
   * @param credentials the credentials for logging the user with a LoginUser Object
   * @returns an Observable of AuthResponse for the HTTP POST function
   */
  loginUser(credentials: LoginUser): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(this.loginUserURL, credentials).pipe(
      tap(user => this.currentUser.set(user))
    );
  }

  /**
   * 
   * @returns the repsone message from logging out a user
   */
  logoutUser(): Observable<LogoutResponse>{
    return this.http.post<LogoutResponse>(this.logoutUserUrl, {}).pipe(
      tap(() => this.currentUser.set(null))
    );
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
