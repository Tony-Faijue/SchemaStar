import { Component } from '@angular/core';
import { DashboardSearchComponent } from "../dashboard-search-component/dashboard-search-component";
import { DashboardSchemaList } from "../dashboard-schema-list/dashboard-schema-list";

@Component({
  selector: 'app-dashboard-main',
  imports: [DashboardSearchComponent, DashboardSchemaList],
  templateUrl: './dashboard-main.html',
  styleUrl: './dashboard-main.css',
})
export class DashboardMain {

}
