import { Component, inject, signal } from '@angular/core';
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

  //Error message for UI
  errorMessage = signal<string | null>(null);

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
      this.logger.log('User attempt login: ', {email});

      this.authenticationService.loginUser(this.loginForm).subscribe({
        next: (response) => {
          //Log the successful login
          this.logger.log('Login successful', {user : response.publicId, email : response.email});
          //redirect to dashboard url'
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          //401 Unauthorized error
          if (err.status === 401){
            this.logger.warn('Failed login attempt: Invalid credentials', {
              email: this.loginForm.value.email
            });
          } else {
            this.logger.error('Login operation failed due to server error', err);
          }
        }
        });
      } else {
        //Get all errors in the FormGroup
        const allErrors = Object.keys(this.loginForm.controls).map(key => {
          return {control: key, errors: this.loginForm.get(key)?.errors};
        }).filter(x => x.errors !== null);
        
        this.logger.warn('Login attempt failed: Form is invalid', allErrors);
        //getControllerError messages show up
        this.loginForm.markAllAsTouched();
      } 
    }

    /**
     * Function to return error message for the controls in the form group
     * @param controlName 
     * @returns string message about the error or null
     */
    getControlError(controlName: string): string|null{
      const control = this.loginForm.get(controlName);
      if (control?.touched && control.errors) {
        if (control.errors['required']) return 'Field is required';
        if (control.errors['email']) return 'Invalid email format';
        if (control.errors['pattern']) return 'Password must include an uppercase, a number and a special character';
        if (control.errors['minlength']) return 'Minimum 8 characters required';
      }
      return null;
    }
}
