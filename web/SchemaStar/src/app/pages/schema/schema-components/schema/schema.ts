import { Component, inject, signal, ViewChild } from '@angular/core';
import {FCanvasComponent, FCreateConnectionEvent, FFlowModule, FZoomDirective} from '@foblex/flow';
import { FlowStateService } from '../../../../services/flow-state-service';
import { EdgeRequest, EdgeType } from '../../../../services/schema/edge-service';
import { MapDataService } from '../../../../services/map-data-service';
import { LoggerService } from '../../../../services/logger-service';
import { SchemaUiStateService } from '../../../../services/schema-ui-state-service';
import { SchemaQuickToolbar } from "../schema-quick-toolbar/schema-quick-toolbar";

@Component({
  selector: 'app-schema',
  imports: [FFlowModule, SchemaQuickToolbar],
  templateUrl: './schema.html',
  styleUrl: './schema.css',
})
export class Schema {
  /**
   * ViewChild Looks for the FCanvas element in HTML
   */
  @ViewChild(FCanvasComponent, {static: true})
  protected fCanvas!: FCanvasComponent;
  
  @ViewChild(FZoomDirective, {static: true})
  protected fZoom!: FZoomDirective;

  public flowStateService = inject(FlowStateService);
  public schemaUiStateService = inject(SchemaUiStateService);
  private mapDataService = inject(MapDataService);
  private loggerService = inject(LoggerService);

  /**
   * Function to handle passing Edge Request data to handleConnectionCreation method in flowstate service
   * @param event 
   */
  public onCreateConnection(event: FCreateConnectionEvent): void {
    const defaultRequest: Partial<EdgeRequest> = {
      edgeType: EdgeType.Undirected,
      uiMetadata: JSON.stringify({color: '#FFFFFF'})
    };
    //Call handlesCreateConnection with default values
    this.flowStateService.handlesCreateConnection(event, defaultRequest as EdgeRequest);
  }

  /**
   * Load the schema and resources for the user 
   */
  ngOnInit(){
    this.mapDataService.loadUserWorkspace();
    //Register the fcanvas and fzoom with schemauistate service
    this.schemaUiStateService.setCanvas(this.fCanvas, this.fZoom);
  }
}
