import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormGroup, FormControl, ReactiveFormsModule, Validators, ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthenticationService, RegisterUser } from '../../services/authentication-service';

@Component({
  selector: 'app-register',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {

  authenticationService = inject(AuthenticationService);
  private router = inject(Router);

  /**
   * 
   * @param control reperesents a FormGroup
   * @returns null if the passwords match or validation error if does not
   */
  passowrdMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null =>{
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
  }, {validators: this.passowrdMatchValidator});

  /**
   * Calls the AuthenticationService to register a new user
   */
  registerUser(): void{
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
        console.log('Registration Successfull!', response);
        //redirect to login page
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Registration', err);
        alert('Registration Failed');
      }
    });
  }  
}
