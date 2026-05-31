import { Component, inject, Input } from '@angular/core';
import { SchemaUiStateService } from '../../../../../../services/schema-ui-state-service';

@Component({
  selector: 'app-view-menu',
  imports: [],
  templateUrl: './view-menu.html',
  styleUrl: './view-menu.css',
})
export class ViewMenu {
  schemaUiStateService = inject(SchemaUiStateService);
  @Input() container!: HTMLElement;  //needed for toggleBrowserFullScreen()
}
