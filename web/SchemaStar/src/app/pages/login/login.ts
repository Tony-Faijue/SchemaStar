import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from "@angular/router";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import { AuthenticationService, LoginUser } from '../../services/authentication-service';
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

  // List of error messages
  errorList = signal<string[]>([]);

  /**
   * Calls the AuthenticationService for logging in the existing user
   */
  login(){
    if (this.loginForm.valid){

      this.logger.log('Login process started');

      //Clear error List
      this.errorList.set([]);

      //Handle the form and send the data to log in the user
      const formData = this.loginForm.getRawValue();
      const credentials: LoginUser = {
        email: formData.email,
        password: formData.password
      };

      this.authenticationService.loginUser(credentials).subscribe({
        next: (response) => {
          //Log the successful login
          this.logger.log('Login successful', {user : response.publicId});
          //redirect to dashboard url'
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          //401 Unauthorized error
          if (err.status === 401){
            this.logger.warn('Failed login: Invalid credentials');
          } else {
            this.logger.error('Login failed: Server Error', {status: err.status}); //Log the error type, not the sensitive input
          }
        }
        });
      } else {
        //getControllerError messages show up
        this.loginForm.markAllAsTouched();

        //Get all errors in the FormGroup
        const allErrors: string[] = [];
        
        //Map each of the controls with array of errors for that control and then display them
        Object.keys(this.loginForm.controls).forEach(key => {
          const fieldErrors = this.getControlErrors(key);
            fieldErrors.forEach(msg => {
              const label = key.charAt(0).toUpperCase() + key.slice(1);
              allErrors.push(`${label}: ${msg}`);
            });
        });

        this.errorList.set(allErrors);
        this.logger.debug('Login prevented: Form validation failed', allErrors);
      } 
    }

    /**
     * Function to return string array of error messages for the controls in the form group
     * @param controlName 
     * @returns string array of errror messages
     */
    getControlErrors(controlName: string): string[]{
      const control = this.loginForm.get(controlName);
      const errors: string[] = [];

      if (control?.touched && control.errors) {
        if (control.errors['required']) errors.push('Field is required');
        if (control.errors['email']) errors.push('Invalid email format');
        if (control.errors['pattern']) errors.push('Password must include an uppercase, a number and a special character');
        if (control.errors['minlength']) errors.push('Minimum 8 characters required');
      }
      return errors;
    }
}
