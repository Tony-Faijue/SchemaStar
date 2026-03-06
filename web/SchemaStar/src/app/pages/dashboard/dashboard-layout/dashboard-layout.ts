import { Component } from '@angular/core';
import { DashboardSideMenu } from "../../../navigation-menus/dashboard-side-menu/dashboard-side-menu";
import { UserMenu } from "../../../navigation-menus/user-menu/user-menu";
import { UserMenuDrawer } from "../../../navigation-menus/user-menu/user-menu-drawer/user-menu-drawer";
import { DashboardMain } from "../dashboard-components/dashboard-main/dashboard-main";
import { CreateSchema } from "../dashboard-components/create-schema/create-schema";
import { CreateSchemaForm } from "../dashboard-components/create-schema-form/create-schema-form";

@Component({
  selector: 'app-dashboard-layout',
  imports: [DashboardSideMenu, UserMenu, UserMenuDrawer, DashboardMain, CreateSchema, CreateSchemaForm],
  templateUrl: './dashboard-layout.html',
  styleUrl: './dashboard-layout.css',
})
export class DashboardLayout {
  isUserMenuOpen: boolean = false;
  isCreateSchemaFormOpen: boolean = false;
}
