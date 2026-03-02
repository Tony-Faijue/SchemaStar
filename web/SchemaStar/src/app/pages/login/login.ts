import { Component, inject } from '@angular/core';
import { RouterLink } from "@angular/router";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import { AuthenticationService } from '../../services/authentication-service';

@Component({
  selector: 'app-login',
  imports: [RouterLink,ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  authenticationService = inject(AuthenticationService);

  //FormGroup  
  loginForm: FormGroup = new FormGroup({
    email: new FormControl('', { 
      nonNullable: true,
      validators: [Validators.maxLength(255), Validators.required, Validators.email]
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.minLength(8), Validators.maxLength(255), Validators.required, Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$/)]
    })
  });

  /**
   * Calls the AuthenticationService for logging in the existing user
   */
  login(){
    if (this.loginForm.valid){
      this.authenticationService.loginUser(this.loginForm).subscribe({
        next: (response) => {
          console.log('Login Successful!', response);
        },
        error: (err) => {
          console.error('Login failed', err);
          alert('Invalid email or password');
        }
        });
      }
    }
}
