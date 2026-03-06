import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-create-schema-form',
  imports: [],
  templateUrl: './create-schema-form.html',
  styleUrl: './create-schema-form.css',
})
export class CreateSchemaForm {
  @Input() isOpen = false;
  @Output() closeForm = new EventEmitter<void>();

  /**
   * emit the closeForm event emitter to the dashboard layout component
   */
  close(){
    this.closeForm.emit();
  }

}
