import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormGroup, FormControl, ReactiveFormsModule, Validators, ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthenticationService, RegisterUser } from '../../services/authentication-service';
import { LoggerService } from '../../services/logger-service';

@Component({
  selector: 'app-register',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {

  //Logger Service
  private logger = inject(LoggerService);

  authenticationService = inject(AuthenticationService);
  private router = inject(Router);

  /**
   * 
   * @param control reperesents a FormGroup
   * @returns null if the passwords match or validation error if does not
   */
  passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null =>{
    const password = control.get('password')!;
    const repassword = control.get('repassword')!;

    if (password.value === repassword.value){
      return null; //Password Matches
    } 
    else {
      return { passwordMismatch: true }; //Password Does not Match return validation error
    }
  }

  /**
   * Register FormGroup to register User Data
   */
  registerForm: FormGroup = new FormGroup({
    username: new FormControl('', { 
      nonNullable: true,
      validators: [Validators.maxLength(255), Validators.required]
    }),
    email: new FormControl('', {
       nonNullable: true,
       validators: [Validators.maxLength(255), Validators.required, Validators.email]
      }),
    password: new FormControl('', {
       nonNullable: true,
       validators: [Validators.minLength(8), Validators.maxLength(255), Validators.required, Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$/)]
      }),
    repassword: new FormControl('', { 
      nonNullable: true,
      validators: [Validators.minLength(8), Validators.maxLength(255), Validators.required, Validators.pattern(/^(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$/)]
    })
  }, {validators: this.passwordMatchValidator});

  // List of error messages
  errorList = signal<string[]>([]);

  /**
   * Calls the AuthenticationService to register a new user
   */
  registerUser(): void{

    if (this.registerForm.valid){

      this.logger.log('Registration process started');
        
      //Clear error List
      this.errorList.set([]);

      //Use of getRawValue() to return the object with strict types
      const formData = this.registerForm.getRawValue();
      const credentials: RegisterUser = {
        username: formData.username,
        email: formData.email,
        password: formData.password
      };

      //Call Authentication service to register the user
      this.authenticationService.registerUser(credentials).subscribe({
        next: (response) => {
          this.logger.log('Registration Successfull!', { user : response.publicId});
          //redirect to login page
          this.logger.log('Redirecting to Login');
          this.router.navigate(['/login']);
        },
        error: (err) => {
            const errorMsg: string[] = [];
          if (err.status == 409){
            this.logger.warn('Registration Failed: Credentials already in use');
            errorMsg.push('Credentials already exists for this account');
          } else if (err.status == 400){
              this.logger.warn('Registration Failed: Registration Validation Failed');
              errorMsg.push('Could not validate Registration with these credentials');
          }
          //getControllerError messages show up
          this.registerForm.markAllAsTouched();
          this.errorList.set(errorMsg);
        }
      });
    } else { 
        //getControllerError messages show up
        this.registerForm.markAllAsTouched();

        //Get all errors in the FormGroup
        const allErrors: string[] = [];

        //Map each of the controls with array of errors for that control and then display them
        Object.keys(this.registerForm.controls).forEach( key => {
          const fieldErrors = this.getControlErrors(key);
            fieldErrors.forEach(msg => {
              const label = key.charAt(0).toUpperCase() + key.slice(1);
              allErrors.push(`${label}: ${msg}`);
            });
        });

        // add Form Errors
        const formErrors = this.registerForm.errors;
        if (formErrors && formErrors['passwordMismatch']){
          allErrors.push('Passwords: The passwords provided do not match');
        }

        this.errorList.set(allErrors);
        this.logger.debug('Registration Prevented: Form validation failed', allErrors);
    }
  }

  /**
     * Function to return string array of error messages for the controls in the form group
     * @param controlName 
     * @returns string array of errror messages
     */
    getControlErrors(controlName: string): string[]{
      const control = this.registerForm.get(controlName);
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
