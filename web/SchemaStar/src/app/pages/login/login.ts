import { Component } from '@angular/core';
import { RouterLink } from "@angular/router";
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [RouterLink,ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

//FormGroup  
loginForm: FormGroup = new FormGroup({
  email: new FormControl(''),
  password: new FormControl('')
});

login(){
  //Call authentication service for logging in the user
  console.log('Login Successful');
  console.log('Email: ', this.loginForm.controls['email'].value)
}
}
