import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { GuestNavigation } from '../navigation-menus/guest-navigation/guest-navigation';
import { FooterMenu } from "../navigation-menus/footer-menu/footer-menu";

@Component({
  selector: 'app-main-layout',
  imports: [RouterModule, GuestNavigation, FooterMenu],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css',
})
export class MainLayout {

}
