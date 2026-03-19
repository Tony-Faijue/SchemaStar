import { Component, inject } from '@angular/core';
import { Router, RouterLink } from "@angular/router";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import { AuthenticationService } from '../../services/authentication-service';
import { LoggerService } from '../../services/logger-service';

@Component({
  selector: 'app-login',
  imports: [RouterLink,ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  //Logger service
  private logger = inject(LoggerService);

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
      //Log the login attempt
      const email = this.loginForm.get('email')?.value;
      this.logger.info('User attempt login: ', {email});

      this.authenticationService.loginUser(this.loginForm).subscribe({
        next: (response) => {
          //Log the successful login
          this.logger.info('Login successful', {user : response.publicId, email : response.email});
          //redirect to dashboard url'
          this.router.navigate(['/dashboard']);
        }
        });
      }
    }
}
