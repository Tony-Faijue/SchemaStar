import { Component, inject } from '@angular/core';
import { SchemaUiStateService } from '../../../../services/schema-ui-state-service';

@Component({
  selector: 'app-schema-toolbar',
  imports: [],
  templateUrl: './schema-toolbar.html',
  styleUrl: './schema-toolbar.css',
})
export class SchemaToolbar {

  schemaUiStateService = inject(SchemaUiStateService);

  /**
   * Sets the active menu state
   * @param menu 
   */
  public setActiveMenu(menu: string){
    this.schemaUiStateService.setActiveMenuSchemaToolBar(menu);
  }

  /**
   * Clears the active menu state
   */
  public clearActiveMenu(){
    this.schemaUiStateService.setActiveMenuSchemaToolBar();
  }
  
}
