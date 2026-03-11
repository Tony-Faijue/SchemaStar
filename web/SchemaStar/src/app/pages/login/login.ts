import { Component, inject } from '@angular/core';
import { Router, RouterLink } from "@angular/router";
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
  private router = inject(Router);

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
          //authenticate the user and update the global state of current user
          this.authenticationService.currentUser.set(response);
          console.log('Login Successful!', response);
          //redirect to dashboard url'
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          console.error('Login failed', err);
          alert('Invalid email or password');
        }
        });
      }
    }
}
