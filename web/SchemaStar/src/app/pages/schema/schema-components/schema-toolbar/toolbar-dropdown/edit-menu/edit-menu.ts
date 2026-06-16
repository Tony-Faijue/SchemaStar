import { Component, inject } from '@angular/core';
import { SchemaUiStateService } from '../../../../../../services/schema-ui-state-service';

@Component({
  selector: 'app-edit-menu',
  imports: [],
  templateUrl: './edit-menu.html',
  styleUrl: './edit-menu.css',
})
export class EditMenu {
  schemaUiStateService = inject(SchemaUiStateService);
}
