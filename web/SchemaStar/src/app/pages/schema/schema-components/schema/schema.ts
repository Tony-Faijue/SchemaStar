import { Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
import {FCanvasComponent, FCreateConnectionEvent, FFlowModule, FZoomDirective} from '@foblex/flow';
import { FlowStateService } from '../../../../services/flow-state-service';
import { EdgeRequest, EdgeType } from '../../../../services/schema/edge-service';
import { MapDataService } from '../../../../services/map-data-service';
import { LoggerService } from '../../../../services/logger-service';
import { SchemaUiStateService } from '../../../../services/schema-ui-state-service';
import { SchemaQuickToolbar } from "../schema-quick-toolbar/schema-quick-toolbar";
import { SchemaToolbar } from "../schema-toolbar/schema-toolbar";

@Component({
  selector: 'app-schema',
  imports: [FFlowModule, SchemaQuickToolbar, SchemaToolbar],
  templateUrl: './schema.html',
  styleUrl: './schema.css',
  host: {
    //-----------------Global Shortcuts/Hot-Keys for Schema Component-----------

    //Zoom and View Shortcuts
    '(document:keydown.control.=)': 'onZoomIn($event)',
    '(document:keydown.control.-)': 'onZoomOut($event)',
    '(document:keydown.control.alt.p)': 'onFitScreenView($event)',
    '(document:keydown.control.alt.o)': 'onResetView($event)',
    '(document:keydown.control.alt.k)': 'onToggleBrowserFullScreen($event)',   
  },
})
export class Schema {
  /**
   * ViewChild Looks for the FCanvas element in HTML
   */
  @ViewChild(FCanvasComponent, {static: true})
  protected fCanvas!: FCanvasComponent;
  
  @ViewChild(FZoomDirective, {static: true})
  protected fZoom!: FZoomDirective;

  /**
   * The #flowContainer div reference
   */
  @ViewChild('flowContainer', {static: true})
  protected flowContainer!: ElementRef<HTMLElement>;

  public flowStateService = inject(FlowStateService);
  public schemaUiStateService = inject(SchemaUiStateService);
  private mapDataService = inject(MapDataService);
  private loggerService = inject(LoggerService);

  //----------------------------------------------HotKeys/Shortcuts---------------------------------------------------------

  /**
   * Calls the zoomIn() function from SchemaUiStateService
   * @param event
   */
  public onZoomIn(event: Event){
    (event as KeyboardEvent).preventDefault();
    this.schemaUiStateService.zoomIn();
  }

  /**
   * Calls the zoomOut() function from SchemaUiStateService
   * @param event 
   */
  public onZoomOut(event: Event){
    (event as KeyboardEvent).preventDefault();
    this.schemaUiStateService.zoomOut();
  }

  /**
   * Calls the onFitScreenView() function from SchemaUiStateService
   * @param event
   */
  public onFitScreenView(event: Event){
    (event as KeyboardEvent).preventDefault();
    if((event as KeyboardEvent).repeat) return;
    this.schemaUiStateService.onFitScreenView();
  }

 /**
   * Calls the onResetView() function from SchemaUiStateService
   * @param event
   */
  public onResetView(event: Event){
    (event as KeyboardEvent).preventDefault();
    if((event as KeyboardEvent).repeat) return;
    this.schemaUiStateService.onResetView();
  }  

   /**
   * Calls the toggleBrowserFullScreen() function from SchemaUiStateService
   * @param event
   */
  public onToggleBrowserFullScreen(event: Event){
    (event as KeyboardEvent).preventDefault();
    if((event as KeyboardEvent).repeat) return;
    this.schemaUiStateService.toggleBrowserFullScreen(this.flowContainer.nativeElement);
  }  

  //----------------------------------------------Core Functionalities---------------------------------------------------------

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
