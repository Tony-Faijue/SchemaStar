import { Component, inject } from '@angular/core';
import { SchemaUiStateService } from '../../../../services/schema-ui-state-service';

@Component({
  selector: 'app-schema-quick-toolbar',
  imports: [],
  templateUrl: './schema-quick-toolbar.html',
  styleUrl: './schema-quick-toolbar.css',
})
export class SchemaQuickToolbar {
 schemaUiStateService = inject(SchemaUiStateService);
}
