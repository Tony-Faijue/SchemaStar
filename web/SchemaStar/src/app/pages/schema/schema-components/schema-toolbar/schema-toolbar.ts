import { Component, inject, Input } from '@angular/core';
import { SchemaUiStateService } from '../../../../services/schema-ui-state-service';
import { ViewMenu } from "./toolbar-dropdown/view-menu/view-menu";
import { EditMenu } from './toolbar-dropdown/edit-menu/edit-menu';

@Component({
  selector: 'app-schema-toolbar',
  imports: [ViewMenu, EditMenu],
  templateUrl: './schema-toolbar.html',
  styleUrl: './schema-toolbar.css',
})
export class SchemaToolbar {

  schemaUiStateService = inject(SchemaUiStateService);
  @Input() container!: HTMLElement; // receives the container html wrapper element for the canvas

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
