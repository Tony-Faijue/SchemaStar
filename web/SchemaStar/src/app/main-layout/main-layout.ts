import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { GuestNavigation } from '../navigation-menus/guest-navigation/guest-navigation';

@Component({
  selector: 'app-main-layout',
  imports: [RouterModule, GuestNavigation],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css',
})
export class MainLayout {

}
