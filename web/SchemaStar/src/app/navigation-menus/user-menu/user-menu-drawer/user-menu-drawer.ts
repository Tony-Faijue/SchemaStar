import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-user-menu-drawer',
  imports: [],
  templateUrl: './user-menu-drawer.html',
  styleUrl: './user-menu-drawer.css',
})
export class UserMenuDrawer {

  @Input() isOpen = false; //state of the drawer from the parent
  @Output() closeDrawer = new EventEmitter<void>(); //tell the parent to close the drawer

  /**
   * emit the closeDrawer event emitter to the parent component (dashboard layout component)
   */
  close(){
    this.closeDrawer.emit();
  }
}
