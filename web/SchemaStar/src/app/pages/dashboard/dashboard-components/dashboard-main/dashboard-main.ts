import { Component } from '@angular/core';
import { DashboardSearchComponent } from "../dashboard-search-component/dashboard-search-component";

@Component({
  selector: 'app-dashboard-main',
  imports: [DashboardSearchComponent],
  templateUrl: './dashboard-main.html',
  styleUrl: './dashboard-main.css',
})
export class DashboardMain {

}
