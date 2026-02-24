import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface RegisterUser{
  username: string,
  email: string,
  password: string
}

export interface LoginUser{
  username: string,
  email: string,
  password: string
}

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {

  http = inject(HttpClient);
}
