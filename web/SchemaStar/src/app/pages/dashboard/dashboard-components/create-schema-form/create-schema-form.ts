import { Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoggerService } from '../../../../services/logger-service';
import { RegisterSchema, SchemaService } from '../../../../services/schema/schema-service';

@Component({
  selector: 'app-create-schema-form',
  imports: [ReactiveFormsModule],
  templateUrl: './create-schema-form.html',
  styleUrl: './create-schema-form.css',
})
export class CreateSchemaForm {
  @Input() isOpen = false;
  @Output() closeForm = new EventEmitter<void>();

  private router = inject(Router);
  private logger = inject(LoggerService);
  schemaService = inject(SchemaService);

  errorList = signal<string[]>([]);

  /**
   * Schema FormGroup
   */
  schemaForm: FormGroup = new FormGroup ({
    schemaName: new FormControl('', {
      nonNullable: true,
      validators: [Validators.minLength(1), Validators.maxLength(255), Validators.required]
    })
  });

  /**
   * Calls the Schema Service to create a new Schema
   */
  createSchema(){

    if (this.schemaForm.valid){
      this.logger.log("Schema creation process started.");
      
      this.errorList.set([]);
      
      //Get raw values
      const formData = this.schemaForm.getRawValue();
      const data: RegisterSchema = {
        nodeWebName: formData.schemaName
      };

      //Call Schema service to create the Schema
      this.schemaService.createSchema(data).subscribe({
        next: (response) => {
          this.logger.log('Schema Creation Successfull', {schema: response.publicId, name: response.nodeWebName});
          //redirect (goes to latest created schema)
          this.logger.log('Redirecting to schema: ', response.publicId);
          this.router.navigate(['/schema']); // redefine routing later
        },
        error: (err) => {
          const errorMsg: string[] = [];
          if (err.status == 409){
            this.logger.warn('Schema Creation Failed: Schema already exists with this name.');
            errorMsg.push('Schema Creation Failed: Schema name already exists.');
          } else if (err.status == 401){
            this.logger.warn('User does not have permission to create Schema');
            errorMsg.push('User unauthorized to create Schema');
          }
          //getControllerError messages show up
          this.schemaForm.markAllAsTouched();
          this.errorList.set(errorMsg);
        }
      });
    } else {
      //getControllerError messages showup
      this.schemaForm.markAllAsTouched();

      //Get all errors in the FormGroup
      const allErrors: string[] = [];

      //Map each of the controls with array of errors for that control and then display them
      Object.keys(this.schemaForm.controls).forEach( key => {
        const fieldErrors = this.getControlErrors(key);
          fieldErrors.forEach(msg => {
            const label = key.charAt(0).toUpperCase() + key.slice(1);
            allErrors.push(`${label}: ${msg}`);
          });
      });
      this.errorList.set(allErrors);
      this.logger.debug('Schema Creation Prevented: Form validation failed', allErrors);
    }
  }

/**
  * Function to return string array of error messages for the controls in the form group
  * @param controlName 
  * @returns string array of errror messages
  */
  getControlErrors(controlName: string): string[]{
    const control = this.schemaForm.get(controlName);
    const errors: string[] = [];

    if (control?.touched && control.errors) {
      if (control.errors['required']) errors.push('Field is required');
      if (control.errors['minlength']) errors.push('Minimum 1 characters required');
      if (control.errors['maxlength']) errors.push('Maximum 255 characters required');
    }
    return errors;
  }

  /**
   * emit the closeForm event emitter to the dashboard layout component
   */
  close(){
    this.closeForm.emit();
  }

}
