import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs'; 
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

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {

  //The url api endpoint for login and registeration
  private registerUserURL = `${SecretData.baseuUrl}/api/users`;
  private loginUserURL = `${SecretData.baseuUrl}/api/users/token`;

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

}
