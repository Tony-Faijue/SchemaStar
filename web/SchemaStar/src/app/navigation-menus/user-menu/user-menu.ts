import { Component, computed, inject } from '@angular/core';
import { AuthenticationService } from '../../services/authentication-service';

@Component({
  selector: 'app-user-menu',
  imports: [],
  templateUrl: './user-menu.html',
  styleUrl: './user-menu.css',
})
export class UserMenu {
  authService = inject(AuthenticationService);
  userName = computed(() => this.authService.currentUser()?.username ?? 'Guest');
  id = computed(() => this.authService.currentUser()?.publicId ?? 'N/A');
}
