import { Component } from '@angular/core';
import { DashboardSideMenu } from "../../../navigation-menus/dashboard-side-menu/dashboard-side-menu";
import { UserMenu } from "../../../navigation-menus/user-menu/user-menu";
import { UserMenuDrawer } from "../../../navigation-menus/user-menu/user-menu-drawer/user-menu-drawer";

@Component({
  selector: 'app-dashboard-layout',
  imports: [DashboardSideMenu, UserMenu, UserMenuDrawer],
  templateUrl: './dashboard-layout.html',
  styleUrl: './dashboard-layout.css',
})
export class DashboardLayout {
  isUserMenuOpen: boolean = false;
}
