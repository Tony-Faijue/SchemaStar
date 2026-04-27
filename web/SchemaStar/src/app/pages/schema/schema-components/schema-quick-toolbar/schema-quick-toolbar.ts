import { Component, inject, input } from '@angular/core';
import { SchemaUiStateService } from '../../../../services/schema-ui-state-service';

@Component({
  selector: 'app-schema-quick-toolbar',
  imports: [],
  templateUrl: './schema-quick-toolbar.html',
  styleUrl: './schema-quick-toolbar.css',
})
export class SchemaQuickToolbar {
  /**
   * Container HTML element for the f-flow root
   */
  public container = input.required<HTMLElement>();
  
  schemaUiStateService = inject(SchemaUiStateService);
}
